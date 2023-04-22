const assert = require('assert');
const WebSocket = require('ws');
const { spawn } = require('child_process');

// Create a test suite for your WebSocket server
describe('WebSocket server', function() {
    let ws;
    let serverProcess;
  
    before(async () => {
        serverProcess = spawn('node', ['index.js']);
        serverProcess.stdout.on('data', (data) => { console.log(data.toString()); });
        serverProcess.stderr.on('data', (data) => { console.error(data.toString()); });

        console.log("Test server spawned");
        await new Promise(resolve => setTimeout(resolve, 1000));
    });

    after(() => {
        serverProcess.kill();
    });

    // Before each test, create a WebSocket connection to the server
    beforeEach(function(done) {
        ws = new WebSocket('ws://localhost:5001');
        done();
    });
  
    // After each test, close the WebSocket connection
    afterEach(function(done) {
      if (ws.readyState === WebSocket.OPEN) {
        ws.on('close', function() {
          done();
        });
        ws.close();
      } else {
        done();
      }
    });

    it('should connect', function(done) {
        ws.on('open', function() {
            done(); 
        });
    });

    /*
    it('Should not login', function(done) {
      ws.on('open', function() {        
        ws.send(JSON.stringify({"action":"LOGIN", uuid: "UUID_TEST_NOT_EXISTS", nick: "UUID_TEST_NOT_EXISTS", token: "NONSENSE", login_type: "HC_LOGIN"}));
        ws.on('message', function(message) {
          assert.equal(message, 'ERR');
          done();
        });
        
      });
    });
    */

    it('Should login', function(done) {
      ws.on('open', function() {        
        ws.send(JSON.stringify({"action":"LOGIN", uuid: "UUID_TEST_NOT_EXISTS", nick: "UUID_TEST_NOT_EXISTS", token: "NONSENSE", login_type: "HC_LOGIN"}));
        ws.on('message', function(message) {
          assert.equal(message, 'ERR');
          done();
        });
        
      });
    });

    /*
    // Write your tests using the WebSocket connection to send and receive messages
    it('should respond to a connection request', function(done) {
      ws.on('message', function(message) {
        assert.equal(message, 'Welcome to the WebSocket server!');
        done();
      });
    });
    */
  });