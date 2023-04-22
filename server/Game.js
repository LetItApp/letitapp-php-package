const Player = require('./Player.js');
const Auth = require('./Auth.js');
const HCServer = require('./HCServer.js');
const RoomManager = require('./RoomManager.js');

const authServerURL = "https://gate.hiddenchicken.com";
const authServerSecret = "gm7fm37g1h876ik87468ui7pzui68z7e68fas6d4axcv5724D";
const gameName = "SLOVNI_DUEL";

class Game{
	constructor(){
		this.hcServer = new HCServer(authServerURL, authServerSecret, gameName);
		
		this.rm = new RoomManager();
		this.rm.setHCServer(this.hcServer);
		this.conns = {};
		this.onlineIDs = {};

		this.clientID = 1000;
		this.roomID = 1000;
		this.connID = 1000;
		this.pongMessageStr = "{'action':'PONG'}";

		this.roomTimeouts = [];
	}

	onConnection(conn){
		conn.SERVER_LOGGED = false;
		this.conns[this.connID++] = conn;
	}
	
	onServerStart(){
		this.hcServer.serverStart();
	}
	
	onClose(conn){
		console.log("Player has closed connection");
		
		if("SERVER_PLAYER" in conn && "uuid" in conn.SERVER_PLAYER){
			delete this.onlineIDs[conn.SERVER_PLAYER.uuid];
			this.rm.removePlayer(conn.SERVER_PLAYER);
			this.hcServer.goOffline(conn.SERVER_PLAYER.uuid);			
			console.log(conn.SERVER_PLAYER.uuid + "/" + conn.SERVER_PLAYER.nick);
		}
	}
	
	onMessage(conn, str){
		try{
			var json_data = JSON.parse(str);
			if(!conn.SERVER_LOGGED && json_data.action !== undefined && json_data.action == "LOGIN"){
				//login
				var player = new Player(json_data.uuid, json_data.nick);
				player.setConn(conn);
				player.setClientID(this.clientID++);
				
				if("reconnect_token" in json_data)
				{
					console.log("reconnecitng token detected");
					player.setReconnectToken(json_data.reconnect_token);
					
					if("reconnecting" in json_data && json_data.reconnecting){
						console.log("reconnecitng");
						
						if(json_data.uuid in this.onlineIDs){
							console.log("player found");
							
							if(this.onlineIDs[json_data.uuid] == json_data.reconnect_token)
							{
								console.log("token match!");
								
								conn.SERVER_PLAYER = this.onlineIDs[json_data.uuid];
								this.onlineIDs[json_data.uuid].setConn(conn);
								conn.SERVER_LOGGED = true;
								player.authSuccessCallback();
								conn.send("OK");
								return;
							}							
						}
					}
				}
				
				conn.SERVER_PLAYER = player;
				conn.SERVER_LOGGED = false;
				
				var auth = new Auth(authServerURL, json_data.login_type, gameName, json_data.uuid, json_data.nick, json_data.token);
				player.setAuth(auth);
				var that = this;			
				auth.asyncAuth(function(){
					conn.SERVER_LOGGED = true;				
					conn.send("OK");
					player.authSuccessCallback();
					that.onlineIDs[json_data.uuid] = player;
					if("SERVER_PLAYER" in conn && "uuid" in conn.SERVER_PLAYER)
						that.hcServer.goOnline(conn.SERVER_PLAYER.uuid);
				}, function(){
					conn.send("ERR");
				});				
				/*if(json_data.login_type == "GOOGLE"){
					//TODO auth verify
					var player = new Player(json_data.uuid, json_data.nick);
					player.setConn(conn);
					player.setClientID(this.clientID++);				

					conn.SERVER_LOGGED = true;
					conn.SERVER_PLAYER = player;

					conn.send("OK");
				}else{
					conn.send("ERR");
				}*/
			}
			else
			if(conn.SERVER_LOGGED){
				var json_data = JSON.parse(str);
				
				if(json_data.action == "WRITE_SCORE"){
					this.hcServer.sendScore(json_data.uuid, json_data.score, function(){}, function(){});
				}
				if(json_data.action == "PREMIUM_TRANSACTION"){
					//var value = Math.abs(json_data.value);
					var value = 1;
					this.hcServer.premiumTransaction(conn.SERVER_PLAYER.info.uuid, value,
						function(){
							//send stats
							conn.SERVER_PLAYER.updateServerProfile(function(player){
								var jsonStr = {"action":"PROFILE_UPDATE", "info": player.info, "premium":true };
								conn.send(JSON.stringify(jsonStr));
							});
						}, 
						function(){
							
					});
				}
				if(json_data.action == "CHECK_FRIEND_ONLINE"){
					var that = this;
					conn.send(JSON.stringify({"action":"FRIEND_STATUS_RESULT", "online": (json_data.friend_id in that.onlineIDs) }));
				}
				if(json_data.action == "UPDATE_PROFILE"){
					conn.SERVER_PLAYER.updateServerProfile(function(player){
						var jsonStr = {"action":"PROFILE_UPDATE", "info": player.info };
						conn.send(JSON.stringify(jsonStr));
					});
				}
				if(json_data.action == "FRIEND_REQUEST"){
					if(this.onlineIDs[json_data.uuid] !== undefined){
						var jsonStr = {"action":"FRIEND_REQUEST" };
						this.onlineIDs[json_data.uuid].conn.send(JSON.stringify(jsonStr));
					}
				}else
				if(json_data.action == "FRIEND_REQUEST_RESULT"){
					if(this.onlineIDs[json_data.uuid] !== undefined){
						   var jsonStr = {"action":"FRIEND_REQUEST_RESULT" };
						   this.onlineIDs[json_data.uuid].conn.send(JSON.stringify(jsonStr));
					}
				}else
				if(json_data.action == "JOIN_ROOM_RANDOM"){
					this.rm.joinRandomGame(conn.SERVER_PLAYER);
				}else
				if(json_data.action == "JOIN_ROOM_PRIVATE_MATCH"){
					var expectedOponents = json_data.expected_oponents;
					var room = this.rm.joinPrivateMatch(conn.SERVER_PLAYER, expectedOponents);
					
					if(room.expectedOponents != null && room.founder == conn.SERVER_PLAYER.getUUID()){
						for(var i = 0; i < room.expectedOponents.length; i++){
							var jsonStr = {"action":"GAME_INVITE", "nick": conn.SERVER_PLAYER.nick, "uuid": conn.SERVER_PLAYER.getUUID(), "room_id": room.getID() };
							this.onlineIDs[room.expectedOponents[i]].conn.send(JSON.stringify(jsonStr));
						}
					}
				}else
				if(json_data.action == "GAME_REQUEST_DENIED"){
					var roomID = parseInt(json_data.room_id);
					var playerToKick = this.onlineIDs[json_data.player];
					if(playerToKick != null){
						this.rm.gameRequestDenied(conn.SERVER_PLAYER, roomID, playerToKick);
					}
				}else		
				if(json_data.action == "PING"){
					conn.SERVER_PLAYER.setLastPing(new Date());
					conn.send(this.pongMessageStr);
				}else
				if(json_data.action == "GAME_START_CONFIRM"){
					var roomID = parseInt(json_data.room_id);
					this.rm.startConfirm(conn.SERVER_PLAYER, roomID);
					this.rm.print();
				}else
				if(json_data.action == "LEAVE_ROOM"){
					var roomID = parseInt(json_data.room_id);
					this.rm.leaveRoom(conn.SERVER_PLAYER, roomID);
					this.rm.print();
				}

				if(json_data.action == "ROOM_PAYLOAD"){
					var roomID = parseInt(json_data.room_id);
					this.rm.roomPayload(conn.SERVER_PLAYER, roomID, json_data.payload);
				}
			}		
		}catch(e){
			console.error(e);
		}
	}
	printRooms(){
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

module.exports = Game;  
