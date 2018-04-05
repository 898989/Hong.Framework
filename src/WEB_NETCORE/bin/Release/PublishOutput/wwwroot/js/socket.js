var SocketConfig = {
    HeartTime : 4,          //心跳间隔时间（单位秒）
    NoOptionDiscount : 4    //无操作断开时间(单位分钟)
}

function Socket(option) {
	this.config = {
		callBak : null,
		concurrent : false,
		heartCall : null,
		pageActive : null,
		pageHidden : null
	};
	for(var item in option){
		this.config[item] = option[item];
	}
	
    this.cmdFinished = false;
    this.lastcmd = "";
	this.socket = null;
	this.connectStatus = 0;
	this.lastSendTime = new Date();
	this.index = 0;
	this.hiddenTimer = -1;
	var _this = this;
	var websocketAction = "";//"Service.ashx";
	
	this.connect = function() {
		if(_this.connectStatus > 0) {
			return;
		}
		
		_this.connectStatus = 1;
		_this.socket = null;
		_this.log("开始连接");
		
		var url = "ws://" + window.location.hostname + "/" + websocketAction;
		if (window.location.port) {
			url = "ws://" + window.location.hostname + ":" + window.location.port + "/" + websocketAction;
		}
		
		var ws = new WebSocket(url);
		ws.errorCount = 0;
		ws.disCount = false;
		ws.heartBack = false;
		ws.identity = Math.random();
		
		ws.onopen = function () {
		    _this.log("打开连接:" + ws.identity);
			ws.healthHeart();
		}

		ws.onmessage = function (evt) {
			ws.errorCount = 0;
			
			if (evt.data && evt.data.split(',')[0] == "1") {
			    _this.log("心跳:" + _this.index++);
			    ws.heartBack = true;
				if(_this.config.heartCall) {
					_this.config.heartCall();
				}
				
				return;
			}
			
			_this.log("收到消息" + _this.index++);
			if (!/^[\{\[]{1}/.test(evt.data)) {
				eval("var result = '" + evt.data + "'");
				alert(evt.data);
				return;
			}
			
			eval("var result = " + evt.data);
			if (!result || result.length == 0) {
				console.log("返回错误");
			}

			if (result.cmd && _this.lastcmd == result.cmd) {
				_this.cmdFinished = true;
			}

			if (result.num == -1002) {
			    document.location.href = "/index.aspx";
				return;
			} else if (result.num == -1000) {
				console.log(result.msg);
				return;
			} else if (result.num == -1003) {
				console.log(result.msg);
				document.location.href = "/index.aspx";
				return;
			} else if (result.num == 0) {
				console.log("请重新登陆");
				document.location.href = "/index.aspx";
				return;
			}

			if (result.cmd == "clear") {
			    document.location.href = "/index.aspx";
				return;
			}

			if (_this.config.callBak) {
				_this.config.callBak(result);
			}
		}

		ws.onerror = function (evt) {
		    _this.log("错误");
			ws.errorCount++;
			
			if (ws.errorCount > 1) {
				console.log(JSON.stringify(evt));
				return;
			}

			ws.disCount = true;
			
			if (ws && ws.readyState == 1) {	
				try{ws.close();} catch(err) {}
			}
		}

		ws.onclose = function () {
		    ws.disCount = true;
		    _this.log("关闭连接" + ws.identity);
		}
		
		ws.healthHeart = function() {
			if (ws.disCount) {
				return;
			}

			if (new Date() - _this.lastSendTime > SocketConfig.NoOptionDiscount * 60000) {
			    ws.disCount = true;
			    return;
			}
			
			ws.heartTimer = setTimeout(function() {
				ws.heartBack = false;
				setTimeout(function() {
					if (ws.heartBack) {
						return;
					}

					_this.log("断开连接" + ws.identity);
					ws.safeClose();
				},1000);
				
				if (ws.readyState != 1) {
					ws.disCount = true;
					return;
				}
				
				ws.send("1");$("#h_nav_title").text("发送心跳");

				ws.healthHeart();
			}, 1000 * SocketConfig.HeartTime);
		}
		
		ws.cancelHeart = function() {
			if (ws.heartTimer) {
				clearTimeout(ws.heartTimer);
			}
		}
		
		ws.safeClose = function() {
			ws.cancelHeart();
			ws.disCount = true;
			
			if (ws.readyState > 0) {
				_this.log("关闭连接" + ws.identity);
				try{
					_this.socket.close();
				} catch(err) {
					_this.log("关闭错误" + ws.identity);
				}
			}
		}
				
		_this.socket = ws;
		_this.connectStatus = 0;
	}
	
	this.close = function() {
		_this.socket.safeClose();
	}

	this.log = function (msg) {
	    console.log(msg);
	    //$("#h_nav_title").text(msg);
		$("#events").append("<div>" + msg + "</div>");
	}
	
    this.waitForConnection = function (cmd, param, callback, interval) {	
        if (!_this.socket || _this.socket.disCount) {
            _this.connect();
        }

        if (_this.socket.readyState === 1) {
            callback(cmd, param);
        } else {
            var that = this;
            setTimeout(function () {
                that.waitForConnection(cmd, param, callback, interval);
            }, interval);
        }
    };

    this.send = function (cmd, param) {
        _this.lastSendTime = new Date();
        _this.waitForConnection(cmd, param, function (cmd, param) {
            if (_this.config.concurrent && !_this.cmdFinished) {
                console.log("请稍候...");
                return;
            }

            _this.lastcmd = cmd;
            _this.cmdFinished = false;

            if (param) {
                _this.socket.send(cmd + ":" + param);
            } else {
                _this.socket.send(cmd + ":");
            }
        }, 20);
    }
	
	this.pageVisibilityChange = function() { 
		var state = document.visibilityState;
		if(state == "hidden"){
			_this.log("页面在后台标签页中或者浏览器最小化");
			if (_this.hiddenTimer) {
				clearTimeout(_this.hiddenTimer);
			}
			
			_this.hiddenTimer = setTimeout(function() {
				_this.close();
				if (_this.config.pageHidden) {
					_this.config.pageHidden();
				}
				
				_this.hiddenTimer = -1;
			}, 30000);

		} else if(state == "visible"){
			_this.log("页面在前台标签页中");
			
			if (_this.hiddenTimer == -1) {
				_this.connect();
			}
			
			if (_this.config.pageActive) {
				_this.config.pageActive();
			}
		} else if(state =="prerender"){
			_this.log("页面在屏幕外执行预渲染处理 document.hidden 的值为 true");
			
		} else if(state =="unloaded"){
			_this.log("页面关闭");
			
		} else {
			_this.log("未知状态");
		}
	}
	
	this.init = function() {
		if(typeof document.visibilityState == "string") {
			document.addEventListener('visibilitychange', _this.pageVisibilityChange);
		} else {
			_this.log("你的浏览器不支持页面事件");
		}
		
		_this.connect();
	}
	
	this.init();
}