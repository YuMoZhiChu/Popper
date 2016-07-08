/* 管理在线玩家的一个列表 */
// 玩家列表如下
// the player list store the imformation like this

// playerList = [player1, player2, player3...]

/*
player : {
	port        :  int 50000 - 60000                    the port number in back, correspond to each player
	ip          :  the ip for the player                such as 127.0.0.1
	idmessage   :  id + '-' + roleNum 
		id      :  int 0 - playerList.length-1;         the _id code in playerlist
		roleNum :  int 0-2 

	name        :  String;                              the flag to define the player online or offline
	score       :  int;                                 the score that player get, 0 at first
	//isOnline  :  bool;                                false at first
	isWait      :  bool;
	isGame      :  bool;
	roomId      :  int;                                 the id of the room which player in
	character   :  int;                                 the num to record character
	messageList :  [message1, message2, message3...]
}

message1 : {
	messageString : string                       
}
*/

var playerList = [];
var basicPortNum = 50000;

var playerWaitString = "wait";
var playerStartString = "start";
var playerChangeScoreString = "change_score";
var playerReturnScoreString = "return_score";
var playerLogoutString = "logout";

// get the playerList
function getPlayerList() {
	return playerList;
}

// get playerList.length
function getPlayerListLength() {
	return playerList.length;
}

function clearPlayerList() {
	playerList = [];
}

// to debug, to monitor the playerList
function showAllPlayers() {
	console.log('在线玩家列表：' + playerList.length)
	playerList.forEach(function(item) {
		console.log(item)
	})
}

function checkeDulName(name, id) {
	var flag = false;
	for (var i = 0; i < playerList.length; i++) {
		if (name == playerList[i].name) {
			flag = true;
			break;
		}
	}
	if (flag) return name + id.toString();
	else return name;
}

function getNewPlayerID() {
	var newID = parseInt((Math.random() * 1000));
	while (true) {
		var flag = false;
		for (var i = 0; i < playerList.length; i++) {
			if (playerList[i].id == newID) flag = true;
		}
		if (!true) break;
		newID = parseInt((Math.random() * 1000));
	}
	return newID;
}

function getIndexByID(id) {
	var index = 0;
	for (var i = 0; i < playerList.length; i++) {
		if (playerList[i].id == id) {
			index = i;
			break;
		}
	}
	return index;
}

// new a player in playerList
function newPlayerInPlayerList(name, ip, callback) {
	// console.log('在玩家列表中新建玩家')

	var newPlayer = {};
	newPlayer.id = getNewPlayerID();
	newPlayer.port = basicPortNum + newPlayer.id;
	newPlayer.ip   = ip;
	newPlayer.name = checkeDulName(name, newPlayer.id);
	newPlayer.roomId = -1;
	
	newPlayer.messageList = [];

	newPlayer.score = 0;
	newPlayer.isWait = false;
	newPlayer.isGame = false;

	playerList.push(newPlayer);

	callback(newPlayer.id, newPlayer.port, newPlayer.name);
}

function newMessageInPlayer(messageString) {
	// console.log('在玩家列表中新建消息')

	var newMessage = {};
	newMessage.messageString = messageString;

	return newMessage;
}

function getPlayerRoomId(id, callback) {
	var flag = false;
	var roomId = -1;

	if (true) {
		flag = true;
		roomId = playerList[getIndexByID(id)].roomId;
	}

	callback(flag, roomId);	
}

// find a player from playerList
function findPlayerById(id, callback) {
	// console.log('根据玩家id查询玩家列表中的玩家')

	var flag = false;
	var player = {};
	if (true) {
		flag = true;
		player = playerList[getIndexByID(id)];
	}

	callback(flag, player);
}
// delete a player from playerList
function deletePlayerById(id, callback) {
	// console.log('根据玩家id查询玩家列表中的玩家')

	var flag = false;
	var roomId = 0;
	if (true) {
		flag = true;
		index = getIndexByID(id);
		roomId = playerList[index].roomId;
		delete playerList[index];
	}

	callback(flag, id);
}

// change a player isWait status by id
function changePlayerIsWaitStatusById(id, newStatus, callback) {
	// console.log('修改玩家isGame状态')

	var flag = false;
	if (true) {
		flag = true;
		playerList[getIndexByID(id)].isWait = newStatus;
	}

	callback(flag, id);
}

// change a player isGame status by id
function changePlayerIsGameStatusById(id, newStatus, callback) {
	// console.log('修改玩家isGame状态')

	var flag = false;
	if (true) {
		flag = true;
		playerList[getIndexByID(id)].isGame = newStatus;
	}

	callback(flag);
}

// change a player roomId by id
function changePlayerRoomIdById(id, roomId, callback) {
	var flag = false;
	if (true) {
		flag = true;
		playerList[getIndexByID(id)].roomId = roomId;
	}

	callback(flag);
}

// change a player score by id
function changePlayerScoreById(id, detalScore, callback) {
	var flag = false;
	if (true) {
		flag = true;
		playerList[getIndexByID(id)].score += detalScore;
	}

	callback(flag);
}

// except the id corresponding to the player
function addMessageByPlayerId(id, message, callback) {

	for (var i = 0; i < playerList.length; i++) {
		if (i != id && playerList[i].roomId == playerList[getIndexByID(id)].roomId) {
			playerList[i].messageList.push(message);
		}
	}

	callback();
}

// udp send interface

function UDPSendInterface(updSendFunction) {
	for (var i = 0; i < playerList.length; i++) {
		var message = new Buffer("");
		var port = playerList[i].port;
		var ip = playerList[i].ip;

		if (playerList[i].messageList.length != 0) {
			message = new Buffer(playerList[i].messageList[0].messageString);
			playerList[i].messageList = playerList[i].messageList.splice(1, playerList[i].messageList.length);
			updSendFunction(message, port, ip);
		}
	}
}

exports.playerListOp = {
	getPlayerList 	       : getPlayerList,
	getPlayerListLength    : getPlayerListLength,
	getPlayerRoomId        : getPlayerRoomId,
	clearPlayerList        : clearPlayerList,
	showAllPlayers 	       : showAllPlayers,
	newPlayerInPlayerList  : newPlayerInPlayerList,
	newMessageInPlayer     : newMessageInPlayer,
	findPlayerById         : findPlayerById,
	deletePlayerById       : deletePlayerById,
	changePlayerIsGameStatusById   :  changePlayerIsGameStatusById,
	changePlayerIsWaitStatusById   :  changePlayerIsWaitStatusById,
	changePlayerRoomIdById : changePlayerRoomIdById,
	changePlayerScoreById  : changePlayerScoreById,
	addMessageByPlayerId   : addMessageByPlayerId,
	UDPSendInterface       : UDPSendInterface,

	playerWaitString        : playerWaitString,
	playerStartString       : playerStartString,
	playerLogoutString      : playerLogoutString,
	playerChangeScoreString : playerChangeScoreString,
	playerReturnScoreString : playerReturnScoreString
}
