//'use strict';

const http = require('http');
const WebSocket = require('ws');
const Game = require('./Game.js');

const server = http.createServer();
const SERVER_PORT = process.env.PORT || 5001;
const wss = new WebSocket.Server({ server:server, clientTracking:true });
var game = new Game();
game.onServerStart();

function noop() {}
function heartbeat(){
  this.isAlive = true;
}

class DebugConn{
	constructor(conn){
		this.conn = conn;
		this.readyStateChanged();
	}
	readyStateChanged(){
		this.readyState = this.conn.readyState;
	}
	send(data){
		var now = new Date();
		var timestamp = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
		console.log("SERVER SAYS("+timestamp+"): " + data);
		this.conn.send(data);
	}
}

wss.on('connection', function connection (conn){	
		var connD = new DebugConn(conn);
		game.onConnection(connD);

		conn.isAlive = true;
		conn.on('pong', heartbeat);

		conn.on('close', function() {
			console.log('echo-protocol Connection Closed');
			connD.readyStateChanged();
			game.onClose(connD);
		});
		conn.on('error', function(error) {
			console.log("Connection Error: " + error.toString());
			connD.readyStateChanged();
			game.onClose(connD);
		});
		conn.on('message', function message(str){
			str = str.toString();

			var now = new Date();
			var timestamp = now.getHours() + ":" + now.getMinutes() + ":" + now.getSeconds();
			console.log("SERVER RECV("+timestamp+"): " + str);
			connD.readyStateChanged();
			game.onMessage(connD, str);
		});	
});

const interval = setInterval(function ping() {
  wss.clients.forEach(function each(ws) {
    if (ws.isAlive === false) return ws.terminate();

    ws.isAlive = false;
    ws.ping(noop);
  });
}, 5 * 60000);

server.listen(SERVER_PORT)
console.log(`Server started on port ${SERVER_PORT}`);

