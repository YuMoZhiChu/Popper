/* 把所有路由汇合的总路由，直接被app使用 */
var playerRouter = require('./playerRouter')
var roomRouter = require('./roomRouter')

var express = require('express');
var router = express.Router();

// 第一级目录
router.use('/player', playerRouter);
router.use('/room', roomRouter);

module.exports = router;