var pictureOp = require('../components/pictureOp.js').pictureOp;
var stringOp = require('../components/stringOp.js').stringOp;
var playerListOp = require('../components/playerListOp.js').playerListOp;

//---------------------------------------------------------------------------

function getClientIp(req) {
    var ipString = req.headers['x-forwarded-for'] ||
    req.connection.remoteAddress ||
    req.socket.remoteAddress ||
    req.connection.socket.remoteAddress;
    var ipStringArray = ipString.split(':');
    if (ipStringArray.length == 4) 
    	return ipStringArray[3];
    else 
    	return "127.0.0.1";
};

// return the player's id, and port
exports.playerRegister = function(req, res) {
	//if (playerListOp.getPlayerListLength() > 2)
	//	return "Please Waiting!"
	var json = req.body.json;
	var obj;	
	try {
		obj = eval("("+json+")");
	}
	catch(exception) {
		console.log(exception);
	}

	var playerIp = getClientIp(req);
	var playerName = obj.playerName;

	playerListOp.newPlayerInPlayerList(playerName, playerIp, function(id, port, name){
		// console.log("新建玩家成功");
		res.send({
			id, port, name
		})
	});

	playerListOp.showAllPlayers();
}

exports.playerLogin = function(req, res) {
	res.send("hello");
}

// get id from front
// call before game end
exports.playerLogout = function(req, res) {
	var json = req.body.json;
	var obj;	
	try {
		obj = eval("("+json+")");
	}
	catch(exception) {
		console.log(exception);
	}

	var playerId = obj.playerId;

	playerListOp.changePlayerIsGameStatusById(playerId, false, function(isSucceed){
		res.send({
			isSucceed
		})
	});

	// playerListOp.showAllPlayers();
}
