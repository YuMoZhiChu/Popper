var pictureOp = require('./components/pictureOp.js').pictureOp;
var stringOp = require('./components/stringOp.js').stringOp;
var playerListOp = require('./components/playerListOp.js').playerListOp;
var UDPSystem = require('./UDPSystem/udp.js').UDPSystem;

//---------------------------------------------------------------------------


/* 注意中间件用法，因为express已更新到4.13.3版本。中间件已经和express框架分开。 */
var express = require('express');
var dgram = require('dgram');
var app = express();
var router = require('./routers');	// 总路由，默认找到index.js文件也就是总路由
var mongoose = require('mongoose');
var bodyParser = require('body-parser');
var connectMultiparty = require('connect-multiparty')
var cookieParser = require('cookie-parser');
var session = require('express-session');
var MongoStore = require('connect-mongo')(session);

// 连接数据库
mongoose.connect('mongodb://localhost/test');

// 设置路由中间件
app.use(bodyParser.urlencoded({	// x-www-form-urlencoded
	extended : true
}));
app.use(bodyParser.json());		// application/json
app.use(connectMultiparty());	// form-data
app.use(cookieParser('my secret cookie'));
app.use(session({
	secret : 'my session',
	store  : new MongoStore({
		mongooseConnection : mongoose.connection,
	}),
	resave : true,
  	saveUninitialized : true,
}));

// 渲染模板
app.set('views', './page/');
app.set('view engine', 'jade');

// 设置各种路由，不过只需要（引用路由文件夹）直接用总路由就好
app.use('/', router);

// 设置端口
var portNumber = 80;
var server = app.listen(portNumber, function () {
	var host = server.address().address;
	var port = server.address().port;
	console.log('Server listening on: ' + port);
});

UDPSystem.SystemOn();
