var Dictionary = function(filename){
	var lineReader = require('readline').createInterface({
	  input: require('fs').createReadStream(filename)
	});

	var allWords = [];
	var startLetterDict = {};

	lineReader.on('line', function (line) {
		allWords.push(line);
		if(!(line[0] in startLetterDict)){
			startLetterDict[line[0]] = [];
		}
		startLetterDict[line[0]].push(line);

	});

	this.getRandomSExcluding = function(letter, used){
		if(!(letter in startLetterDict)){
			return null;
		}
		var num = startLetterDict[letter].length;
		var tries = 0;
		while(tries++ < 100){
			var index = Math.floor(num * Math.random());
			if(used.indexOf(startLetterDict[letter][index]) === -1 && startLetterDict[letter][index].length <= 7){
				return startLetterDict[letter][index];
			}
		}
	}

	this.hasWord = function(w){
		return allWords.indexOf(w) !== -1;
	}
	
	var ready = false;
	
	this.isReady = function(){
		return ready;
	}
	
	lineReader.on('close',  function(){	
		ready = true;
	});
}

var dict = new Dictionary('dictionaries/Czech.txt');

var BotPlayer = function(){
	this.dict = dict;
	this.usedWords = [];

	this.init = function(){
		this.usedWords = [];
	}

	//var nameArr = ["Jan", "Petr", "Pavel", "Lukáš", "Adam", "Karel"];
	//var surnameArr = ["Ježek", "Liška", "Novák", "Navrátil", "Sekera", "Hruška", "Červenka"];

	//this.nick = nameArr[Math.floor(Math.random() * nameArr.length)] + "" + surnameArr[Math.floor(Math.random() * surnameArr.length)];

	var nickNames = ["HiddenChicken", "AsaurusRex", "Captain", "TheDude", "GlitcherSK", "Froggie", "SableCat", "Heliotopia", "ViperStrike", "Quibble", "MyrtleGirl", "Miniscus", "Deadlight", "Sceptre", "OwlChick", "Indira", "Palanquin", "Palanquin", "BatBoy", "MarlingCZ", "OgreMan", "AstroBooy"];	
	this.nick = nickNames[Math.floor(Math.random() * nickNames.length)];
	
	
	var skins = ["default", "fight", "winner", "rose", "banana", "sword"];
	var skin = skins[Math.floor(Math.random() * skins.length)];

	this.uuid = Math.random()*1000000 + "_bot";
	this.info = {
		    "uuid" : this.uuid,
		    "nick" : this.nick,
		    "score" : Math.floor(Math.random() * 1000),
		    "premium_coins" : Math.floor(Math.random() * 100),
		    "made_payment": Math.floor(Math.random() * 2),
		    "experience" : Math.floor(Math.random() * 20),
		    "actual_skin": skin,
		    "coins" : Math.floor(Math.random() * 1000)
	};

	//simulation connection
	this.conn = {
		send:function(data){
			console.log(data);
		}
	}
	this.clientID = -1;
	this.logged = true;

	this.getUUID = function(){
		return this.uuid;
	}
	this.getInfo = function(){
		return this.info;
	}
	this.play = function(letter){
		let w = dict.getRandomSExcluding(letter, this.usedWords);
		this.usedWords.push(w);
		return w;
	}
};

/*
setTimeout(function() {
	var b1 = new BotPlayer(dict);
	for(var i = 0; i < 20; i++)
		console.log(b1.play("h"));
	console.log(b1.nick);
}, 3000);
*/

module.exports = BotPlayer;
