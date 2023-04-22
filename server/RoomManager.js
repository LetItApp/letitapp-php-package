const Room = require('./Room.js');
const Bot = require('./Bot.js');

class RoomManager{
	constructor(){
		this.rooms = {};		
		this.nextRoomID = 0;		
		this.publicRooms = [];
		this.privateRooms = [];
	}
	
	removeFromPrivateRooms(room){
		var index = this.privateRooms.indexOf(room);		
		if(index >= 0){
			this.privateRooms.splice(index, 1);
		}
	}
	
	removeFromPublicRooms(room){
		var index = this.publicRooms.indexOf(room);
		if(index >= 0){
			this.publicRooms.splice(index, 1);
		}		
	}
	
	addToPrivateRooms(room){
		var index = this.privateRooms.indexOf(room);		
		if(index == -1){
			this.privateRooms.push(room);
		}
	}
	
	addToPublicRooms(room){
		var index = this.publicRooms.indexOf(room);
		if(index == -1){
			this.publicRooms.push(room);
		}		
	}
	
	getOrCreateRoom(privateRoom, uuid){
		var room = null;
		
		if(privateRoom){
			for(var i in this.privateRooms){
				if(this.privateRooms[i].isExpecting(uuid)){
					return this.privateRooms[i];
				}
			}
		}else{
			var mostOccupiedRooms = this.publicRooms.sort(function(a, b){ return a.playersCount - b.playersCount; });
			if(mostOccupiedRooms.length > 0){
				return mostOccupiedRooms[mostOccupiedRooms.length - 1];
			}
		}
		
		//not found need to create
		var room = new Room(this.nextRoomID++);
		room.setMaxPlayers(2);
		room.setMinPlayers(2);
		room.founder = uuid;
		this.rooms[room.getID()] = room;
		
		var that = this;
		room.setGameStartFailCallback(function(fRoom){
                 fRoom.kickPlayers();
                 that.removeFromPrivateRooms(fRoom);
                 that.removeFromPublicRooms(fRoom);
        	});
        	
        	if(privateRoom){
        		this.addToPrivateRooms(room);
        	}else{
        		this.addToPublicRooms(room);
        	}
        	
		return room;
	}
	
	setHCServer(hc){
		this.hcServer = hc;
	}
	
	removePlayer(player){
		for(var i in this.rooms){
			if(this.rooms[i].hasPlayer(player)){
				this.rooms[i].removePlayer(player);
				this.destroyRoomWhenEmpty(this.rooms[i]);
			}
		}
	}
	
	joinRandomGame(player){
		var room = this.getOrCreateRoom(false, null);
		room.addPlayer(player);

		(function(cRoom, that){
			var botTimeout = setTimeout(function(){
				if(cRoom.status == "LOBBY" && cRoom.playersCount == 1){
					console.log("Starting bot");
					var bot = new Bot();
					bot.init();
					cRoom.addPlayer(bot);
					console.log(that);			
					that.removeFromPublicRooms(cRoom); //there should be 2 players now					
					cRoom.sendUpdateState();
					cRoom.numConfirms = 1;
					cRoom.checkStart();
					
					var botWords = [];
					var playerWords = [];
					
					var gameRunning = true;
					
					cRoom.setGameEndedCallback(function(){
						gameRunning = false;
						var score = 5 * playerWords.length;
						for(var i in botWords){
							if(playerWords.indexOf(botWords[i]) !== -1){
								score -= 2;
							}
						}
						console.log("Player score: " + score);
						that.hcServer.sendScore(player.uuid, score, function(){}, function(){});

						cRoom.removePlayer(bot);
						that.destroyRoomWhenEmpty(cRoom);
					});
					
					let forcedWords = 1 + Math.random() * 3;
					
					for(let i = 0; i < forcedWords; i++){
						setTimeout(function(){
							if(gameRunning){
								let word = bot.play(cRoom.gamePlayData.instanceLetter.toLowerCase());
								
								let realPlayers = Object.keys(cRoom.players).filter(playerID => cRoom.players[playerID] !== bot);
								let p = realPlayers[0];
								p = cRoom.players[p];
								
								p.conn.send(JSON.stringify({ 'action' : 'WORD_ENTERED', 'word': word }));
								botWords.push(word);
							}						
						}, 11000 + Math.random() * 2000 + i * 5000);
					}		
					
					
					cRoom.setPlayerMessageCallback(function(p, m){
						if(m.target == "ALL" && m.payload.action == "WORD_ENTERED"){
							playerWords.push(m.payload.word);
							let actualLetter = m.payload.word[0];
																					
							if(gameRunning){
								setTimeout(function(){
									let word = bot.play(actualLetter);									
									botWords.push(word);
									
									var pSilent = Math.random();
									var pRespondTwice = Math.random();
									
									if(pSilent > 0.6){
										   return;
									}
									
									p.conn.send(JSON.stringify({ 'action' : 'WORD_ENTERED', 'word': word }));
									
									if(pRespondTwice > 0.5){
										   word = bot.play(m.payload.word[0]);
										   botWords.push(word);
										   p.conn.send(JSON.stringify({ 'action' : 'WORD_ENTERED', 'word': word }));
									}
								}, 1000 + Math.random() * 2000);								
							}
							
							
							
						}
					});     
				}
			}, 25000 + Math.random() * 10000);
		})(room, this);
		
		room.sendUpdateState();
		
		if(room.haveEnoughPlayers()){ //ensure no one else connects
			this.removeFromPublicRooms(room);
		}
		
		room.checkStart();
		this.print();
		return room;
	}
	
	joinPrivateMatch(player, expectedOponents){
		var room = this.getOrCreateRoom(true, player.getUUID());
		
		if(room.founder == player.getUUID())
			room.expectedOponents = expectedOponents;
					
		room.addPlayer(player);
		room.sendUpdateState();
		room.checkStart();
		this.print();
		return room;
	}
	
	roomExists(roomID){
		return roomID in this.rooms;
	}
	
	roomHasPlayer(roomID, player){
		return this.roomExists(roomID) && this.rooms[roomID].hasPlayer(player);
	}
	
	getRoom(roomID){
		return this.rooms[roomID];
	}
	
	gameRequestDenied(player, roomID, playerToKick){
		var room = this.getRoom(roomID);
		if(room != null && room.hasPlayer(playerToKick)){
			room.removePlayer(playerToKick);
			this.destroyRoomWhenEmpty(room);
			
			var jsonStr = {"action":"KICK_GAME_REQUEST_DENIED" };//TODO move to player
			playerToKick.conn.send(JSON.stringify(jsonStr));
		}
	}
	
	destroyRoomWhenEmpty(room){
		if(room != null){
			if(room.playersCount == 0){
				this.removeFromPublicRooms(room);
				this.removeFromPrivateRooms(room);
				
				delete this.rooms[room.getID()];
			}
		}
	}
	
	startConfirm(player, roomID){
		var room = this.getRoom(roomID);
		if(room != null && room.hasPlayer(player)){
			room.playerConfirmsStart(player);
			console.log("Player " + player.nick + " confirmed start");
			this.print();
		}
	}
	
	leaveRoom(player, roomID){
		var room = this.getRoom(roomID);
		if(room != null && room.hasPlayer(player)){
			room.removePlayer(player);
			this.destroyRoomWhenEmpty(room);
			console.log("Player " + player.nick + " leaved room " + room.getID());			
			this.print();
		}
	}
	
	roomPayload(player, roomID, payload){
		var room = this.getRoom(roomID);
		if(room != null && room.hasPlayer(player)){
			room.playerMessage(player, payload);
		}
	}
	
	print(){
		console.log("---------------- ROOMS -------------------");
		for(var i in this.rooms){
			console.log("--------------------------------------------");
			console.log("RoomID: " + this.rooms[i].id);
			console.log("Num players" + this.rooms[i].playersCount);
			console.log("Room Status:" + this.rooms[i].status);
			this.rooms[i].printDebug();
		}
	}
}

module.exports = RoomManager;
