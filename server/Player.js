class Player{
	constructor(uuid, nick){
		this.nick = nick;
		this.uuid = uuid;
		this.conn = null;

		this.lastPing = new Date();
				
		this.clientID = -1;
		this.logged = false;
		
		this.reconnectToken = Math.random();
		
		this.info = {
			"uuid" : this.uuid,
			"nick" : this.nick,
			"score" : 0,
			"premium_coins" : 0,
			"made_payment":0,
			"experience" : 0,
			"actual_skin": "default",
			"coins" : 0
		};
	}
		
	setAuth(auth){
		this.auth = auth;
	}
	
	updateServerProfile(succCallback){
		var that = this;
		this.auth.getScore(this.uuid, function(d){
			//console.log(d);
			if(d.length > 0){
				that.info.score = d[0].Score;
				that.info.coins = d[0].Coins;
				that.info.actual_skin = d[0].ActualSkin;
				that.info.premium_coins = d[0].PremiumCoins;
				that.info.experience = d[0].Experience;
				that.info.made_payment = d[0].MadePayment;
				//console.log("Player stats");
				//console.log(that.info);
				if(succCallback != undefined)
					succCallback(that);
			}			
		}, function(){});
	}
	
	authSuccessCallback(){
		this.updateServerProfile();
	}
	
	setReconnectToken(t){
		this.reconnectToken = t;
	}
	
	setLastPing(v){
		this.lastPing = v;
	}

	isLogged(){
		return this.logged;
	}
	
	setConn(c){
		this.conn = c;
	}
	
	setClientID(id){
		this.clientID = id;
	}
	
	getUUID(){
		return this.uuid;
	}
	
	getInfo(){
		return this.info;
	}
}

module.exports = Player;
