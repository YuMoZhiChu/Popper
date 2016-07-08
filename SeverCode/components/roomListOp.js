/* 管理在线房间的一个列表 */
// 玩家列表如下
// the room list store the imformation like this

// roomList = [room1, room2, room3...]

/*
room : {
	state      :    isGame or not
	id1        :    id for room1
	id2        :    id for room2
	id3        :    id for room3
	           .
	           .
	           .
}

*/

var playerListOp = require('../components/playerListOp.js').playerListOp;

//---------------------------------------------------------------------------

var roomList = [];
var roomSize = 4;
var scoreTableStringFront = "scoreTableString|"

// get the roomList
function getRoomList() {
	return roomList;
}

// get roomList.length
function getRoomListLength() {
	return roomList.length;
}

function clearRoomList() {
	roomList = [];
}

// to debug, to monitor the roomList
function showAllRooms() {
	console.log('在线房间列表：' + roomList.length)
	roomList.forEach(function(item) {
		console.log(item)
	})
}

// new a room in roomList
function newRoomInRoomList(name, ip, callback) {
	// console.log('在玩家列表中新建玩家')

	var newRoom = [];
	newRoom.id = roomList.length; // began as 0
	// newRoom.isGame = false;

	roomList.push(newRoom);

	callback(newRoom.id);
}

// add a player in the a room
function addPlayerInARoom(id, callback) {
	var len = roomList.length;

	if (0 == len || roomList[len-1].length == roomSize) {
		// || roomList[len-1].isGame == true) {

		var newRoom = [];
		newRoom.id = len;
		len = len + 1;
		newRoom.push(parseInt(id));
		roomList.push(newRoom);

	} else {
		roomList[len-1].push(parseInt(id));
		// roomList[len-1].isGame = false;
	}

	callback(len-1);
}

// get the score table in the room
function getRoomScoreTable(id, callback) {
	
	var flag = false;
	var scoreTableString = scoreTableStringFront;
	var room = roomList[id];
	
	for (var i = 0; i < room.length; i++) {
		var player = playerListOp.getPlayerList()[i];
		scoreTableString += player.id.toString() + ":" + player.score.toString() + ",";
	}
	
	callback(true, id, scoreTableString);
}

// start the game in a room
function startGameInARoom(id, callback) {
	var flag = false;

	var room = roomList[id];
	for (var i = 0; i < room.length; i++) {
		playerListOp.changePlayerIsGameStatusById(room[i], true, function(){});
	}

	// roomList[id].isGame = true;

	callback(true, id);
}

// udp send interface

function UDPSendInterfaceForRoom(updSendFunction, roomId, messageString) {

	var playerList = playerListOp.getPlayerList();
	// console.log("send start message to others!");

	for (var i = 0; i < playerList.length; i++) {
		var message = new Buffer("");
		var port = playerList[i].port;
		var ip = playerList[i].ip;
		if (roomId == playerList[i].roomId) {
			message = new Buffer(messageString);
			playerList[i].messageList = playerList[i].messageList.splice(1, playerList[i].messageList.length);
			updSendFunction(message, port, ip);
		}
	}
}

function getRandomArray() {
	var arry = [];
	for (var i = 0; i < 100; i++) {
		arry.push(i);
	}
	
	var iLength = arry.length;
	var i = iLength;
	var mTemp;
	var iRandom;
	
	while(i--) {
		if (i != (iRandom = Math.floor(Math.random() * iLength) )) {
			mTemp = arry[i];
			arry[i] = arry[iRandom];
			arry[iRandom] = mTemp;
		}
	}
	
	//console.log(arry);
	
	return "RandomNum|" + arry.toString();
}

function getLogoutMessage(userId) {
	return "Logout|" + userId.toString();
}

exports.roomListOp = {
	getRoomList 	        :  getRoomList,
	getRoomListLength       :  getRoomListLength,
	clearRoomList           :  clearRoomList,
	showAllRooms 	        :  showAllRooms,
	newRoomInRoomList       :  newRoomInRoomList,
	addPlayerInARoom        :  addPlayerInARoom,
	getRoomScoreTable       :  getRoomScoreTable,
	startGameInARoom        :  startGameInARoom,
	UDPSendInterfaceForRoom :  UDPSendInterfaceForRoom,
	getRandomArray          :  getRandomArray
}

