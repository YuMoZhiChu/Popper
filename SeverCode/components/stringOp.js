var uuid = require('node-uuid')
var crypto = require('crypto')

function getRandomString() {
	return uuid.v1();
}

function getHash(password) {
	//return crypto.createHash('sha1').update(pwd).digest('hex');
	return password;	// for debug
}

function getRandom(length) {
    var chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ'.split('');
    if (!length) length = 32;
    var str = '';
    for (var i = 0; i < length; i++) {
        str += chars[Math.floor(Math.random() * chars.length)];
    }
    return str;
}

function getRandomNumber(length) {
    var chars = '0123456789'.split('');
    if (!length) length = 32;
    var str = '';
    for (var i = 0; i < length; i++) {
        str += chars[Math.floor(Math.random() * chars.length)];
    }
    return str;
}

function getRandomStringByTime() {
    return crypto.createHash('md5')
              .update(new Date().getTime().toString())
              .digest('hex').substr(0, 16);
}

function urlencode(clearString) 
{
    var output = '';
    var x = 0;
     
    clearString = utf16to8(clearString.toString());
    var regex = /(^[a-zA-Z0-9-_.]*)/;
 
    while (x < clearString.length) 
    {
        var match = regex.exec(clearString.substr(x));
        if (match != null && match.length > 1 && match[1] != '') 
        {
            output += match[1];
            x += match[1].length;
        } 
        else
        {
            if (clearString[x] == ' ')
                output += '+';
            else
            {
                var charCode = clearString.charCodeAt(x);
                var hexVal = charCode.toString(16);
                output += '%' + ( hexVal.length < 2 ? '0' : '' ) + hexVal.toUpperCase();
            }
            x++;
        }
    }
 
    function utf16to8(str) 
    {
        var out, i, len, c;
 
        out = "";
        len = str.length;
        for(i = 0; i < len; i++) 
        {
            c = str.charCodeAt(i);
            if ((c >= 0x0001) && (c <= 0x007F)) 
            {
                out += str.charAt(i);
            } 
            else if (c > 0x07FF) 
            {
                out += String.fromCharCode(0xE0 | ((c >> 12) & 0x0F));
                out += String.fromCharCode(0x80 | ((c >> 6) & 0x3F));
                out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
            } 
            else
            {
                out += String.fromCharCode(0xC0 | ((c >> 6) & 0x1F));
                out += String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
            }
        }
        return out;
    }
 
    return output;
}

function getClientIp(req) {
    var ret = req.headers['x-forwarded-for'] ||
                req.connection.remoteAddress ||
                req.socket.remoteAddress ||
                req.ip ||
                req.connection.socket.remoteAddress;
    var obj = ret.match(/\d+\.\d+\.\d+\.\d+/);
    return obj['0'];
}

exports.stringOp = {
	getRandomString : getRandomString,
	getHash			: getHash,
	// urlencode       : urlencode,
    getRandom       : getRandom,
    getRandomNumber : getRandomNumber,
    getClientIp     : getClientIp,
    getRandomStringByTime : getRandomStringByTime,
}
