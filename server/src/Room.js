const WebSocket = require('ws');

class Room{
	constructor(id){
		this.id = id;
		
		this.players = {};
		this.playersCount = 0;
		this.numConfirms = 0;

		this.expectedOponents = [];
		
		this.minPlayers = 0;
		this.maxPlayers = 0;
		
		this.info = {};
		this.timers = [];
		this.info["room_id"] = this.id;
		this.status = "LOBBY";
		this.responseTimer = -1;
		this.gameStartFailCallback = function(obj){};
		this.playerMessageCallback  = function(obj){};
		this.gameEndedCallback = function(obj){};
		this.gamePlayData = {};
	}
	addTimer(t){
		this.timers.push(t);
	}
	printDebug(){
		console.log("ROOM INFO " + this.info);
		for(var i in this.info){
			console.log(i + ":" + this.info[i]);
		}
		for(var i in this.players){
			console.log(this.players[i].uuid + "/" + this.players[i].nick);
		}
		console.log("EXpected oponents: " + this.expectedOponents);
	}

	setPlayerMessageCallback(ev){
		this.playerMessageCallback = ev;
	}

	setGameStartFailCallback(ev){
		this.gameStartFailCallback = ev;
	}

	setGameEndedCallback(ev){
		this.gameEndedCallback = ev;
	}
	kickPlayers(){
                for(var i in this.players){
                	this.removePlayer(this.players[i]);
                }
        }
	reset(){
		this.status = "LOBBY";
		this.numConfirms = 0;
		this.expectedOponents = [];

		//this.gameStartFailCallback = function(obj){};
                this.playerMessageCallback  = function(obj){};
                this.gameEndedCallback = function(obj){};

		this.players = {};
		this.playersCount = 0;
		for(var i in this.timers){
			clearTimeout(this.timers[i]);
		}
		this.timers = [];
	}

	playerConfirmsStart(player){
		console.log("START confirmed");
		this.numConfirms++;
		this.checkStart();
	}
	
	shouldMoveFromLobby(){
		return this.status == "GAME";
	}
	
	setMinPlayers(v){
		this.minPlayers = v;
	}
	
	setMaxPlayers(v){
		this.maxPlayers = v;
	}
		
	getInfo(){
		return this.info;
	}
	
	getID(){
		return this.id;
	}
		
	updateInfo(){
		this.info["players"] = [];		
		for(var i in this.players){
			this.info["players"].push(this.players[i].getInfo());
		}
	}
	
	addPlayer(p){
		if(!(p.getUUID() in this.players)){
			this.players[p.getUUID()] = p;
			this.playersCount++;
			
			if(this.playersCount == this.maxPlayers){ //TODO zapnout po x sekundach když je větší než min
				this.checkStart();
			}
		}		
		
		this.updateInfo();
	}
	
	removePlayer(p){
		if(p.getUUID() in this.players){
			delete this.players[p.getUUID()];
			this.playersCount--;
		}
		
		this.playersCount = Math.max(0, this.playersCount);
		
		console.log("removing player");
		console.log("from room ID" + this.id);
		
		if(this.playersCount == 0){
			this.reset();
		}
		
		this.updateInfo();
	}
	
	isExpecting(uuid){
		return this.expectedOponents != null && this.expectedOponents.indexOf(uuid) != -1;
	}
	
	hasPlayer(p){
		return p != null && p.getUUID() in this.players;
	}
	
	playerMessage(p, msg_json){
		this.playerMessageCallback(p, msg_json);
		if(msg_json.target == "ALL"){
			var payload = JSON.stringify(msg_json.payload);
			
			var myUUID = p.getUUID();
			
			for(var i in this.players){
				if(i != myUUID && this.players[i].conn.readyState === WebSocket.OPEN){
					this.players[i].conn.send(payload);
				}
			}
		}else
		if(msg_json.target == "UUID"){
			var payload = JSON.stringify(msg_json.payload);
			var targetUUID = msg_json.target_uuid;
			
			if(targetUUID in this.players){
				if(this.players[targetUUID].conn.readyState === WebSocket.OPEN){
					this.players[targetUUID].conn.send(payload);
				}
			}
		}
	}
	
	start(){
		this.status = "GAME";
		var instanceSeed = Math.random();
		var alphabet = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "V", "Z", "Č", "Ř", "Š", "Ž"];
		var instanceLetter = alphabet[Math.floor(instanceSeed * alphabet.length)];

		this.gamePlayData.instanceLetter = instanceLetter;

		for(var i in this.players){
			this.sendUpdateState(this.players[i]);	
			this.players[i].conn.send(JSON.stringify({
				"action" : "GAME_START",
				"room_id" : this.id,
				"letter" : instanceLetter,
				"time" : 10, //s
				"game_time":50
			}));
		}
		var that = this;
		setTimeout(function(){
			for(var i in that.players){
				that.players[i].conn.send(JSON.stringify({
					"action" : "GAME_END",
					"room_id" : that.id
				}));
			}
			that.gameEndedCallback();
		}, 60000);
	}
	
	shouldStart(){
		return this.haveEnoughPlayers() && this.status != "GAME";
	}

	haveEnoughPlayers(){
		return this.playersCount >= this.minPlayers;
	}
	
	sendUpdateState(p){
		if(p === undefined){
			p = this.players;
		}else{
			p = [p];
		}
	
		for(var i in p){
			if(p[i].conn.readyState === WebSocket.OPEN){
				p[i].conn.send(JSON.stringify({ "action": "ROOM_INFO", "room_id": this.id, "info": this.getInfo() }));
			}	
		}
	}
	
	checkStart(){
		if(this.shouldStart()){
			if(this.status != "WAIT_FOR_CONFIRM"){
				this.status = "WAIT_FOR_CONFIRM";
				for(var i in this.players){
					this.players[i].conn.send(JSON.stringify({
                                		"action" : "GAME_STARTING",
                                		"room_id" : this.id
                        		}));
				}
				var that = this;
				this.responseTimer = setTimeout(function(){  
					if(that.playersCount != that.numConfirms){
						for(var i in that.players){
							that.removePlayer(that.players[i]);
						}
						that.reset();
						that.gameStartFailCallback(that);
					}
				}, 20000);
			}
			if(this.playersCount == this.numConfirms && this.status == "WAIT_FOR_CONFIRM"){
				if(this.responseTimer != -1)
					clearTimeout(this.responseTimer);

				this.start();
			}
		}
	}
}

module.exports = Room;
