const https = require('https');

class Auth{
	constructor(serverURL, type, game, id, nick, token){
		this.type = type;
		this.game = game; 
		this.userID = id;
		this.nick = nick;
		this.token = token;
		
		this.serverURL = serverURL;
	}
	
	getScore(uuid, successCallback, failCallback){
		var req = {
			"action" : "GET_SCORE",
			"game" : this.game,
			"uuid" : uuid
		};
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
		console.log(reqURL);
		
		https.get(reqURL, (resp) => {
			let data = '';
			resp.on('data', (chunk) => {
				data += chunk;
			});
			resp.on('end', () => {
				try{
					var jsonData = JSON.parse(data);
					//console.log(jsonData);
					successCallback(jsonData);
				}catch(e){
					console.error(e);
				}
			});
		}).on("error", (err) => {
		  console.log("Error: " + err.message);
		  failCallback();
		});
	}
	
	
	asyncAuth(successCallback, failCallback){
		this.successCallback = successCallback;
		this.failCallback = failCallback;
		
		var req = {
			"action" : "LOGIN",
			"nick" : this.nick,
			"login_type" : this.type,
			"auth_token" : this.token,
			"user_id" : this.userID
		};		
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
		console.log(reqURL);
		
		https.get(reqURL, (resp) => {
			let data = '';
			resp.on('data', (chunk) => {
				data += chunk;
			});
			resp.on('end', () => {
				try{
					console.log(data);
					var jsonData = JSON.parse(data);
					console.log(jsonData);
					
					successCallback();
				}catch(e){
					console.error(e);
				}
			});
		}).on("error", (err) => {
		  console.log("Error: " + err.message);
		  failCallback();
		});
	}
}

module.exports = Auth;  
