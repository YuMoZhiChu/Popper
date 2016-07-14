var pictureOp = require('../components/pictureOp.js').pictureOp;
var stringOp = require('../components/stringOp.js').stringOp;
var playerListOp = require('../components/playerListOp.js').playerListOp;
var roomListOp = require('../components/roomListOp.js').roomListOp;

var dgram = require('dgram');

//---------------------------------------------------------------------------

var messageToSend = new Buffer("");
var messageGet = new Buffer("");
var socketPort = 60001;
var socket = dgram.createSocket('udp4');
socket.bind(socketPort);

function SystemOn () {
	console.log("UDPSystem On.");
	console.log('Socket listening on: ' + socketPort);
	MessageGet();
}

function BasicSend() {
	if (playerListOp.getPlayerListLength() == 0) {
		return;
	}
	
}

function MessageGet() {
	socket.on('message', function(msg, rinfo) {
		// console.log('收到 %d 字节，来自 %s:%d\n', msg.length, rinfo.address, rinfo.port);
		messageArray = msg.toString("ASCII", 0, msg.length).split('|');
		// console.log(rinfo);
		// console.log(messageArray);
		
		if (messageArray.length == 2) {
			id = messageArray[0].split('-')[0];
			
			var messageExceptId = new String(messageArray[1]);
			// player wait!
			if (messageExceptId == playerListOp.playerWaitString) {
				playerListOp.changePlayerIsWaitStatusById(id, true, function(flag, id){
					if (!flag) {
						console.log("error command, can not change player is_wait state!");
						return;
					}
				})
				roomListOp.addPlayerInARoom(id, function(roomId){
					playerListOp.changePlayerRoomIdById(id, roomId, function(flag){
						if (!flag) {
							console.log("error command, can not change player roomId!");
						}
					});
				})
				//playerListOp.showAllPlayers();
				//roomListOp.showAllRooms();
				return;
			}

			// player start!
			if (messageExceptId == playerListOp.playerStartString) {
				playerListOp.changePlayerIsGameStatusById(id, true, function(flag, id){
					if (!flag) {
						console.log("error command, can not change player is_game state!");
						return;
					}
				})
				playerListOp.getPlayerRoomId(id, function(flag, roomId){
					if (!flag) {
						console.log("error command, can not find the room of the player");
						return;
					}
					roomListOp.startGameInARoom(roomId, function(flag, roomId){
						if (!flag) {
							console.log("error command, can not start the game in the room");
							return;
						} else {
							roomListOp.UDPSendInterfaceForRoom(function(message, port, ip){
								socket.send(message, 0, message.length, port, ip, function(err){
									// console.log("Send :" + message + " To : " + ip + ":" + port);
								}, id);
							}, roomId, playerListOp.playerStartString);
							
							// send random array
							roomListOp.UDPSendInterfaceForRoom(function(message, port, ip){
								socket.send(message, 0, message.length, port, ip, function(err){
									// console.log("Send :" + message + " To : " + ip + ":" + port);
								}, id);
							}, roomId, roomListOp.getRandomArray());
						}
					})
				})
				
				// player logout
				if (messageExceptId == playerListOp.playerLogoutString) {
					playerListOp.deletePlayerById(id, function(flag, roomId){
					if (!flag) {
						console.log("error command, can not delete player!");
						return;
					} else {
						// send random array
						roomListOp.UDPSendInterfaceForRoom(function(message, port, ip){
							socket.send(message, 0, message.length, port, ip, function(err){
								// console.log("Send :" + message + " To : " + ip + ":" + port);
							}, id);
						}, roomId, roomListOp.getRandomArray(id));
					}
				})
				}

				playerListOp.showAllPlayers();
				roomListOp.showAllRooms();
				return;
			}
			
			// player get score table
			if (messageExceptId == playerListOp.playerReturnScoreString) {
				playerListOp.getPlayerRoomId(id, function(flag, roomId){
					if (!flag) {
						console.log("error command, can not find the room of the player");
						return;
					}
					roomListOp.getRoomScoreTable(roomId, function(flag, roomId, scoreTableMessage){
						if (!flag) {
							console.log("error command, can not get the score table of the room");
							return;
						} else {
							roomListOp.UDPSendInterfaceForRoom(function(message, port, ip){
								socket.send(message, 0, message.length, port, ip, function(err){
									// console.log("Send :" + message + " To : " + ip + ":" + port);
								}, id);
							}, roomId, scoreTableMessage);
						}
					})
				})
			}
			
			
			// player change score
			if (messageExceptId.indexOf(playerListOp.playerChangeScoreString) != -1) {
				// console.log(messageExceptId);
				var changeScoreArray = messageExceptId.split(":");
				// console.log(changeScoreArray);
				if (changeScoreArray.length != 2) {
					console.log("error message in change score commend");
					return;
				} else {
					var scoreString = changeScoreArray[1];
					var detalScore = parseInt(scoreString);
					playerListOp.changePlayerScoreById(id, detalScore, function(flag){
						if (!flag) {
							console.log("could not change detal score!");
							return;
						}
						// playerListOp.showAllPlayers();
					});
				}
			}
			
			playerMessage = msg;
			
			var newMessage = playerListOp.newMessageInPlayer(playerMessage);

			playerListOp.addMessageByPlayerId(id, newMessage, function() {
				// do nothing
			});

			// playerListOp.showAllPlayers();

			playerListOp.UDPSendInterface(function(message, port, ip){
				socket.send(message, 0, message.length, port, ip, function(err) {
					// console.log("Send :" + message + " To : " + ip + ":" + port);
				});
			});
		}
	});
}

exports.UDPSystem = {
	SystemOn : SystemOn
}
