var express = require('express');
var app = express();
var port = 8088;
var io = require('socket.io').listen(app.listen(port));
var room = ['admin','user'];
var users = [];
app.get('/', function(req, res){
  //res.send('<h1>Hello world</h1>');
});
console.log("Listening on port " + port);
io.sockets.on('connection', function(socket){
    
	//var sessionID = socket.id;
	console.log('client: ' + socket.id);
	//join room
	socket.on('join-room',function(obj){
		var userId = obj.userId;
		console.log(userId);
		if(obj.isAdmin){
			console.log('join room admin');
			socket.join('admin');
		}
		else{
			console.log('join room user');
			var existUser = false;
			for(var i = 0;i< users.length ;i++){
				if(users[i].id === userId){
					existUser = true;
					users[i].sessionId = socket.id;
					break;
				}
			}
			if(!existUser){
				console.log('add user to list');
				var user = {};
				user.id = userId;
				user.sessionId = socket.id;
				//add user to aray
				users.push(user);
			}
		}
	});
	
	//send notify to admin
    socket.on('send-to-admin', function(obj){
		console.log('send notify to admin');
		io.to('admin').emit('show-notify', obj);
	});
	
	//send notify to user
    socket.on('send-to-user', function(obj){
		var sessionId = null;
		for(var i = 0;i< users.length ;i++){
			if(users[i].id === obj.userId){
				existUser = true;
				sessionId = users[i].sessionId;
				break;
			}
		}
		console.log('send notify to userid:'+obj.userId);
		console.log('send notify to user:'+sessionId);
		io.to(sessionId).emit('show-notify', obj);
	});
  
  
    
});