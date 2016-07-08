var path = require('path')
	, fs = require('fs')

var avatarDir = path.resolve(__dirname, '../pictures/avatars');		// 用户头像文件夹
var imageDir = path.resolve(__dirname, '../pictures/images');		// 菜单、菜图像文件夹

function saveAvatar(picStrEncodedByBase64, filename, callback) {	// 保存头像
	var buf = new Buffer(picStrEncodedByBase64, 'base64');		// 以base64方式将图片字符串decode成一个buffer
	var filepath = path.resolve(avatarDir, filename);
	fs.writeFile(filepath, buf, function(err) {		// 文件操作最好基于Buffer类对象，就不用写编码方式
		if (err) {
			console.log("Error when writing into an avatar file. Please check the path of file in server.");
			callback(false); 
		} else {
			console.log("Write avatar file OK!");
			callback(true); 
		}
	});
}

function saveImage(picStrEncodedByBase64, filename, callback) {	// 保存菜单等图片
	var buf = new Buffer(picStrEncodedByBase64, 'base64');
	var filepath = path.resolve(imageDir, filename);
	fs.writeFile(filepath, buf, function(err) {
		if (err) {
			console.log("Error when writing into an image file. Please check the path of file in server.");
			callback(false); 
		} else {
			console.log("Write image file OK!");
			callback(true); 
		}
	});
}

function deleteImage(filename, callback) {
	var filepath = path.resolve(imageDir, filename);
	fs.unlink(filepath, function(err) {
		if (err) {
			console.log('Error when deleting a picture of a dish. Please check the function (deleteDish) in the dishController.')
			callback(false);
		} else {
			console.log("delete image file OK!");
			callback(true);
		}
	}) 
}

exports.pictureOp = {
	saveAvatar 	: saveAvatar,
	saveImage 	: saveImage,
	deleteImage : deleteImage,
	avatarDir   : avatarDir,
	imageDir    : imageDir
}