var playerController = require('../controller/playerController.js');
var express = require('express');
var router = express.Router();

router.post('/playerRegister', playerController.playerRegister);
router.get('/playerLogin', playerController.playerLogin);
router.post('/playerLogout', playerController.playerLogout);

module.exports = router;