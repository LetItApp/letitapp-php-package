const https = require('https');

class HCServer{
	constructor(serverURL, secret, game){		
		this.serverURL = serverURL;
		this.secret = secret;
		this.game = game;
	}
	serverStart(){
		var req = {
			"action" : "SERVER_START"
		};
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
                https.get(reqURL, (resp) => {
                        let data = '';
                        resp.on('data', (chunk) => {
                                data += chunk;
                        });
                        resp.on('end', () => {
                                console.log(data);
                                //var jsonData = JSON.parse(data);
                                //console.log(jsonData);

                                //successCallback();
                        });
                }).on("error", (err) => {
                  console.log("Error: " + err.message);
                  //failCallback();
                });
	}
	goOnline(uuid){
		var req = {
			"action" : "GO_ONLINE",
			"uuid" : uuid,
		};
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
                https.get(reqURL, (resp) => {
                        let data = '';
                        resp.on('data', (chunk) => {
                                data += chunk;
                        });
                        resp.on('end', () => {
                                console.log(data);
                                //var jsonData = JSON.parse(data);
                                //console.log(jsonData);

                                //successCallback();
                        });
                }).on("error", (err) => {
                  console.log("Error: " + err.message);
                  //failCallback();
                });
	}
	goOffline(uuid){
		var req = {
			"action" : "GO_OFFLINE",
			"uuid" : uuid,
		};
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
		https.get(reqURL, (resp) => {
                        let data = '';
                        resp.on('data', (chunk) => {
                                data += chunk;
                        });
                        resp.on('end', () => {
                                console.log(data);
                                //var jsonData = JSON.parse(data);
                                //console.log(jsonData);

                                //successCallback();
                        });
                }).on("error", (err) => {
                  console.log("Error: " + err.message);
                  //failCallback();
                });
	}
	premiumTransaction(uuid, value, successCallback, failCallback){
		var req = {
			"action" : "PREMIUM_TRANSACTION",
			"uuid" : uuid,
			"value" : value,
			"game" : this.game,
			"secret" : this.secret
		};
		
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
		console.log(reqURL);
		https.get(reqURL, (resp) => {
			let data = '';
			resp.on('data', (chunk) => {
				data += chunk;
			});
			resp.on('end', () => {
				console.log(data);
				//var jsonData = JSON.parse(data);
				//console.log(jsonData);
				
				successCallback();
			});
		}).on("error", (err) => {
		  console.log("Error: " + err.message);
		  failCallback();
		});
	}
	
	sendScore(uuid, score, successCallback, failCallback){
		//this.successCallback = successCallback;
		//this.failCallback = failCallback;
		
		var req = {
			"action" : "WRITE_SCORE",
			"uuid" : uuid,
			"score" : score,
			"game" : this.game,
			"secret" : this.secret
		};
		var reqURL = this.serverURL + '/?json=' + JSON.stringify(req);
		console.log(reqURL);
		https.get(reqURL, (resp) => {
			let data = '';
			resp.on('data', (chunk) => {
				data += chunk;
			});
			resp.on('end', () => {
				console.log(data);
				var jsonData = JSON.parse(data);
				console.log(jsonData);
				
				successCallback();
			});
		}).on("error", (err) => {
		  console.log("Error: " + err.message);
		  failCallback();
		});
	}
}

module.exports = HCServer;  
