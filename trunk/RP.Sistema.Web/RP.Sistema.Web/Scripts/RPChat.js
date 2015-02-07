var RPChatHub,
    _sendTime = null,
    _searchTime = null;

var RPChat = new function () {
    var _senderId = RPCHATH_SENDERID,
        _senderName = RPCHATH_SENDERNAME,
        _userList = {},
        _chatHistory = {},
        _chatNotificationTime = null,
        _userStatus = { 1: 'status-offline', 2: 'status-online', 3: 'status-busy', 4: 'status-away', 5: 'status-invisible' };

    // configuracoes do chat
    this.config = null;

    this.userList = null;

    // eventos da interface
    this.attachUIEvents = function () {
        $(document).keydown(function (e) {
            if (e.keyCode == $.ui.keyCode.ESCAPE) {
                e.preventDefault()
            }
        });

        $('#user-list-search').keydown(function (e) {
            var $this = $(this);
            var keyCode = $.ui.keyCode;
            var rows, activeNumRows;

            clearTimeout(_searchTime);
            _searchTime = setTimeout(function () {
                if (RPChat.config.ShowOfflineContacts) {
                    rows = RPChat.userList.find('li');
                }
                else {
                    rows = RPChat.userList.find('li:not(.hidden), li.notoff');
                }

                if (e.keyCode == keyCode.ESCAPE) {
                    $this.val('');
                    rows.removeClass('hidden');
                    RPChat.Message(false);
                }
                else {
                    if (e.which) {
                        var regExp = new RegExp($.removeAccents($this.val()), 'i');
                        var name, email;

                        rows.addClass('hidden');
                        if (!RPChat.config.ShowOfflineContacts) {
                            rows.addClass('notoff');
                        }
                        else {
                            rows.removeClass('notoff');
                        }

                        activeNumRows = 0;

                        rows.each(function (i, row) {
                            name = $.removeAccents($(row).find('.user-name').text());
                            email = $.removeAccents($(row).find('small').text());

                            if (regExp.test(name) || regExp.test(email)) {
                                $(row).removeClass('hidden');
                                activeNumRows++;
                            }
                        });

                        if (activeNumRows) {
                            RPChat.Message(false);
                            rows.not(':hidden').last().addClass('no-border');
                        }
                        else {
                            RPChat.Message(true, '<div>Nenhum contato encontrado</div>');
                            if (!RPChat.config.ShowOfflineContacts) {
                                $('<a href="#"/>')
                                    .text('Exibir meus contatos offline')
                                    .click(function (e) {
                                        e.preventDefault();
                                        RPChat.config.ShowOfflineContacts = true;
                                        RPChatHub.server.setUserConfig(RPChat.config).fail(function (e) {
                                            alert(e);
                                        });

                                    })
                                    .appendTo($("#user-list-text-status"));
                            }
                        }
                    }
                }

                RPChat.userList.removeHighlight().highlight($this.val());

            }, 50);
        });

        // eventos de cada usuario da lista
        RPChat.userList.on({
            // ao clicar
            click: function (e) {
                e.preventDefault();
                // se o usuario estiver online
                if (!$(this).hasClass('status-offline') || !$(this).hasClass('status-invisible')) {
                    // recupera o id do usuario
                    var data_userid = $(this).attr('data-userid');
                    RPChat.openChat(_senderId, _senderName, data_userid, _userList[data_userid].Nome, true);
                }
            }
        }, 'li a');

        // eventos do titulo e do label da janela
        $('#dock-modal-chat').on({
            // ao clicar
            click: function (e) {
                e.preventDefault();
                var chatList = $('#dock-modal-chat').find('.modal-chat:not(.collapsed)');
                $(this).parent()
                    .toggleClass('collapsed')
                    .find('textarea:first').setFocus();

                if ($(this).parent().hasClass('collapsed')) {
                    if (chatList.length > 1) {
                        var index = chatList.index($(this).parent());
                        var newIndex = index + 1;

                        if (newIndex > chatList.length - 1) {
                            newIndex = 0;
                        }

                        $(chatList.get(newIndex)).find('textarea:first').setFocus();
                    }
                }

                RPChat.repositionChatUI();
            }
        }, '.chat-label, .chat-header');

        // eventos do corpo e rodape da janela
        $('#dock-modal-chat').on({
            // ao soltar clique do mouse
            mouseup: function (e) {
                // se nao houver nenhum texto selecionado
                if (!Functions.getSelectedText()) {
                    // seta o campo de digitacao da janela
                    $(this).parent()
                        .find('textarea:first').setFocus();
                }
            }
        }, '.chat-body, .chat-footer');

        // evento da janela
        $('#dock-modal-chat').on({
            // ao pressionar alguma tecla
            keydown: function (e) {
                var keyCode = $.ui.keyCode;
                var self = $(this);
                var parent;
                switch (e.keyCode) {
                    // ao pressionar tecla ESC
                    case keyCode.ESCAPE:
                        e.preventDefault();
                        parent = self.parent();
                        self.remove();
                        parent.find('.modal-chat:not(.collapsed):last').find('textarea:first').setFocus();
                        RPChat.repositionChatUI();
                        break;
                }
            },
            // ao clicar
            click: function (e) {
                var target = $(e.target);
                // se o elemento clicado for o botao de fechar a janela
                if (target.is('button') && target.hasClass('close')) {
                    e.preventDefault();
                    var self = target.parents('.modal-chat');
                    var parent = self.parent();
                    self.remove();
                    parent.find('.modal-chat:not(.collapsed):last').find('textarea:first').setFocus();
                    RPChat.repositionChatUI();
                }
            }
        }, '.modal-chat');

        // evento do input/textarea da janela
        $('#dock-modal-chat').on({
            // ao pressionar alguma tecla
            keydown: function (e) {
                var keyCode = $.ui.keyCode;
                switch (e.keyCode) {
                    // ao pressionar tecla TAB
                    case keyCode.TAB:
                        if ($('#dock-modal-chat').find('.modal-chat:not(.collapsed)').length > 1) {
                            e.preventDefault();

                            var chatList = $('#dock-modal-chat').find('.modal-chat:not(.collapsed)');
                            if (chatList.length > 1) {
                                var index = chatList.index($(this).parents('.modal-chat'));
                                var newIndex = index - 1;
                                if (e.shiftKey) {
                                    newIndex = index + 1;
                                }

                                if (newIndex > chatList.length - 1) {
                                    newIndex = 0;
                                }
                                else if (newIndex < 0) {
                                    newIndex = chatList.length - 1;
                                }

                                $(chatList.get(newIndex)).find('textarea:first').setFocus();
                            }
                        }
                        break;

                    // ao pressionar tecla(s) ENTER
                    case keyCode.ENTER:
                    case keyCode.NUMPAD_ENTER:  
                        if (!e.shiftKey) {
                            e.preventDefault();
                            var self = $(this);
                            var toUserId = $.data(self.parents('.modal-chat')[0], 'userId');
                            var messageText = self.val();

                            RPChatHub.server.sendMessage(messageText, toUserId)
                                .fail(function (err) {
                                    RPChat.addRoomMessage(toUserId, _senderId, _senderName, messageText, new Date(), true);
                                });

                            self.val('');
                        }
                        break;
                    default:
                        var c = e.keyCode;
                        var send = (!((c >= 65 && c <= 90) || (c >= 97 && c <= 122) || (c == 32) || (c >= 48 && c <= 57))) ? false : true;

                        if (send && !_sendTime) {
                            RPChat.sendNotification($.data($(this).parents('.modal-chat')[0], 'userId'));
                            _sendTime = setTimeout(function () { clearTimeout(_sendTime); _sendTime = null; }, 1000);
                        }
                }
            },
            // ao dar foco
            focus: function () {
                $(this).parents('.modal-chat').removeClass('highlight-title');
            }
        }, '.chat-footer textarea');
    };

    // eventos do hub
    this.attachEvents = function () {
        // inicia eventos da interface
        RPChat.attachUIEvents();

        $.connection.hub.logging = false;
        RPChatHub = $.connection.RPChatHub;

        $.connection.hub.start()
        .done(function () {
            RPChatHub.server.getUserConfig()
            .done(function (config) {
                RPChat.config = config;
                RPChat.setConfiguration(config);

                RPChatHub.server.getUserContacts()
                .done(function (users) {
                    RPChat.loadUserList(users);
                })
                .fail(function () {
                    console.log('Falha ao carregar contatos');
                });
            })
            .fail(function () {
                console.log('Falha ao carregar status');
            });
        })
        .fail(function (err) {
            alert(err);
        });

        RPChatHub.client.setConfiguration = function (config) {
            RPChat.setConfiguration(config);
        }

        RPChatHub.client.userConnected = function (userId, status) {
            var a = RPChat.userList.find('li a[data-userid="' + userId + '"]');
            a.removeClass().addClass(_userStatus[status]);

            if (!RPChat.config.ShowOfflineContacts) {
                if (status == 1 || status == 5) {
                    a.parent().addClass('hidden');
                }
                else {
                    a.parent().removeClass('hidden');
                }

                var $list = RPChat.userList.find('li');
                $list.removeClass('no-border');
                $list.not(':hidden').last().addClass('no-border');
            }

            $('#sidebar #info-chats-online').text(RPChat.totalOnlineTalkers());
        };

        RPChatHub.client.userReconnected = function () {
            console.log('tentando reconectar');
        };

        RPChatHub.client.userDisconnected = function (userId) {
            var a = RPChat.userList.find('li a[data-userid="' + userId + '"]');
            a.removeClass().addClass(_userStatus[1]);
            $('#sidebar #info-chats-online').text(RPChat.totalOnlineTalkers());
        };

        RPChatHub.client.receiveMessage = function (chatMessage) {
            // abre a janela de conversa
            RPChat.openChat(chatMessage.senderId, chatMessage.senderName, chatMessage.receiverId, chatMessage.receiverName, false);

            // se o usuario da janela for o mesmo que enviou a mensagem
            if (_senderId == chatMessage.senderId) {
                // escreve a mensagem na janela do proprio usuario
                RPChat.addRoomMessage(chatMessage.receiverId, _senderId, _senderName, chatMessage.messageText, chatMessage.timestamp);
            }
            else {
                // escreve mensagem na janela do usuario
                RPChat.addRoomMessage(chatMessage.senderId, chatMessage.senderId, chatMessage.senderName, chatMessage.messageText, chatMessage.timestamp);

                // exibe notificacao para a janela do usuario
                RPChat.addRoomAlert(chatMessage.senderId, chatMessage.senderName);
            }
        };

        // evento ao receber notificao que algum usuario esta escrevendo
        RPChatHub.client.receiveNotification = function (senderId, senderName) {
            // se nao for o proprio usuario
            if (_senderId != senderId) {
                // adiciona notificacao
                RPChat.addNotification(senderId, senderName);
            }
        };
    };

    // carrega a lista de contatos
    this.loadUserList = function (users) {
        var work = new ThreadLoop(users);
        var html = '';

        RPChat.userList.css('margin-top', '-9999px');

        work.action = function (user, index, total) {
            _userList[user.Id] = user;

            if (user.Id != _senderId) {
                html += $.trim(tmpl("tmpl-user-item", {
                    id: user.Id,
                    name: user.Nome,
                    email: user.Email,
                    status: _userStatus[user.Status || 1]
                }));
            }
        };
        work.finish = function (thread) {
            RPChat.userList.append(html);
            RPChat.showOfflineContacts(RPChat.config.ShowOfflineContacts);
            RPChat.userList.css('margin-top', 0);

            RPChat.Message(false);
            if (RPChat.config.Enabled) {
                $("#sidebar-overlay").addClass('hidden');
            }
        };
        work.start();
    };

    this.showOfflineContacts = function (show) {
        RPChat.Message(false);
        var $list = RPChat.userList.find('li');

        $list.find('a.status-offline, a.status-invisible').each(function (i, e) {
            if (show) {
                $(e).parent().removeClass('hidden');
            }
            else {
                $(e).parent().addClass('hidden');
            }
        });

        $list.removeClass('no-border');
        $list.not(':hidden').last().addClass('no-border');
    };

    this.setConfiguration = function (config) {
        RPChat.config = config;
        RPChat.setConfigurationStatus(config.Status);
        RPChat.setConfigurationOnline(config.ShowOfflineContacts);
        RPChat.setConfigurationNotification(config);
    };

    this.setConfigurationStatus = function (status) {
        $('#btn-config-status').removeClass('status-online status-offline status-busy status-away status-invisible').addClass(_userStatus[status]);
        $('[name=status][value='+status+']').attr('checked', true);
        $('[name=status]').renderIcons("reload");
    };

    this.setConfigurationOnline = function (online) {
        $('[name=exibir][value=1]').prop('checked', online);
        $('[name=exibir]').renderIcons("reload");
        RPChat.showOfflineContacts(online);
    }

    this.setConfigurationNotification = function (config) {
        $('[name=notificacao][value=1]').prop('checked', config.AlertSound);
        $('[name=notificacao][value=2]').prop('checked', config.AlertMessage);
        $('[name=notificacao]').renderIcons("reload");
    }

    // retorna a quantidade de usuarios online
    this.totalOnlineTalkers = function () {
        return RPChat.userList.find('li .status-online').length;
    };

    // retorna a quantidade de janelas com conversas ainda nao lidas
    this.totalUnreadChats = function () {
        return $('#dock-modal-chat').find('.modal-chat.highlight-title').length;
    };

    // envia uma mensagem para a sala informada
    this.sendChatMessage = function (receiverId, message, chatRoomId) {
        // se mensagem for nula, para execucao
        if (IsNullOrEmpty(message))
            return;
        // cria estrutura da mensagem
        var chatMessage = {
            senderId: _senderId,
            senderName: _senderName,
            receiverId: receiverId,
            receiverName: _userList[receiverId].Nome,
            conversationId: chatRoomId,
            messageText: message
        };
        // envia mensagem ao servidor de mensagens
        RPChatHub.server.sendChatMessage(chatMessage).fail(function (e) {
            alert(e);
        });
        // por padrao, retorna falso
        return false;
    };

    // envia comando que algo esta sendo escrito
    this.sendNotification = function (receiverId) {
        RPChatHub.server.sendNotification(receiverId);
        return false;
    };

    this.openChat = function (senderId, senderName, receiverId, receiverName, setFocus) {
        // define o usuario a ser exibido na sala de conversa
        var userId = (senderId == _senderId ? receiverId : senderId);
        var userName = (senderName == _senderName ? receiverName : senderName);

        // define e armazena o nome da sala
        var chatId = $('#chat-' + userId);

        // se ja existir uma sala ativa, apenas exibe a mesma
        if (chatId.length > 0) {
            chatId.removeClass('collapsed');
        }
        // senao, cria uma nova janela de conversa e armazena o id da mesma
        else {
            var chatWindow = $($.trim(tmpl("tmpl-chat-window", { userId: userId, name: userName })));
            $.data(chatWindow[0], 'userId', userId);
            $('#dock-modal-chat').append(chatWindow);

            // carrega o historico de mensagens
            RPChatHub.server.getChatMessages($.data(chatWindow[0], 'userId'))
            .done(function (messages) {
                if (messages && messages.length) {
                    $.each(messages, function (i, message) {
                        RPChat.addRoomMessage(userId, message.senderId, message.senderName, message.messageText, message.timestamp);
                    });
                }
            })
            .fail(function () {
                console.log('Falha ao carregar historico das mensagens');
            });
        }

        // reposiciona as janelas ativas
        RPChat.repositionChatUI();

        if (typeof setFocus == 'undefined' || setFocus == true) {
            RPChat.setFocus(userId);
        }
    };

    // reposiciona as janelas de conversa
    this.repositionChatUI = function () {
        var chats = $('#dock-modal-chat .modal-chat');
        if (chats.length > 0) {
            var right = 0;
            $(chats).each(function () {
                $(this).css('right', right);
                right += parseInt($(this).width()) + 7;
            });
        }
    };

    this.updateSize = function () {
        var h = $('#sidebar').height() - 101;
        $('#content-list').height(h).parent().height(h);
    };

    this.Message = function (show, message) {
        var statusContainer = $("#user-list-text-status");
        if (!show) {
            statusContainer.addClass('hidden').text('');
        }
        else {
            statusContainer.html(message).removeClass('hidden');
        }
    };

    // seta o foco no campo de digitacao da janela informada
    this.setFocus = function (chatId) {
        $('#chat-' + chatId).find('textarea:first').TextAreaExpander().setFocus();
    };

    // trata o conteudo da mensagem
    this.prepareMessage = function (message) {
        var msg = Functions.htmlEncode(message);
        return msg.replace(new RegExp("(((f|ht){1}tp://)[-a-zA-Z0-9@:%_\+.~#?&//=]+)", 'gim'), '<a href="$1" target="_blank">$1</a>');
    };

    // adiciona mensagem a janela informada
    this.addRoomMessage = function (chatRoomId, userId, userName, text, timestamp, isFail) {
        // Define variaveis
        var currentChatDate,
            lastChatDate,
            ticks,
            chatGroupMessage,
            chatItemGroupMessage,
            message,
            appendDividerDate = false;

        // obtem o container de mensagens da janela
        var chatBody = $('#chat-' + chatRoomId).find('.chat-body');

        // obtem a data da mensagem
        currentChatDate = new Date(timestamp);
        ticks = currentChatDate.toString('yyyyMMddHHmm');

        // se o container estiver vazio
        if (chatBody.find('li').length == 0) {
            // cria um novo grupo de mensagens
            chatGroupMessage = $($.trim(tmpl('tmpl-chat-item', { id: userId, name: userName, profile_image: (userId == _senderId ? 'alternative' : 'default'), ticks: ticks })));
        }
        else {
            // busca o grupo da ultima mensagem exibida
            chatGroupMessage = chatBody.find('ul li').last();
            // se o grupo não for do mesmo usuario ou mesmo timestamp da mensagem atual
            if ((chatGroupMessage.attr('data-userid') != userId) || (chatGroupMessage.attr('data-ticks') != ticks)) {
                // cria um novo grupo de mensagens
                chatGroupMessage = $($.trim(tmpl('tmpl-chat-item', { id: userId, name: userName, profile_image: (userId == _senderId ? 'alternative' : 'default'), ticks: ticks })));
            }
        }

        // obtem a ultima data do container
        lastChatDate = $.data(chatBody[0], 'lastChatDate');

        // se existir ultima data setada
        if (lastChatDate) {
            // se a data da mensagem atual for maior que a data da ultima mensagem
            if (currentChatDate.clone().clearTime().compareTo(lastChatDate.clone().clearTime()) == 1) {
                // seta variavel para adiciona data no cabecalho
                appendDividerDate = true;
            }
        }
        else {
            // seta variavel para adiciona data no cabecalho
            appendDividerDate = true;
        }

        // seta data atual no container de mensagens
        $.data(chatBody[0], 'lastChatDate', currentChatDate);

        // busca e informa hora da mensagem atual
        chatGroupMessage.find('.chat-info').text(currentChatDate.toString('HH:mm'));

        // obtem a lista de mensagens dentro do grupo
        chatItemGroupMessage = chatGroupMessage.find('.chat-messages');

        // adiciona a mensagem na lista
        message = $('<div/>').text(RPChat.prepareMessage(text));
        if (isFail) {
            message.addClass('chat-message-fail').attr('title', 'Falha ao enviar mensagem');
        }
        chatItemGroupMessage.append(message);

        // se for para adicionar cabecalho
        if (appendDividerDate) {
            // seta como falso
            appendDividerDate = false;
            // adiciona data no cabecalho das mensagens
            chatBody.find('ul').append('<li class="divider">' + currentChatDate.toString('dd MMMM, yyyy') + '</li>');
        }

        // adiciona a lista de mensagens dentro do container
        chatBody.find('ul').append(chatGroupMessage);

        // mantem o scroll da janela sempre em baixo
        chatBody.prop({ scrollTop: chatBody.prop("scrollHeight") });

        // inicia o tooltip do bootstrap
        $('[data-toggle="tooltip"]').tooltip();

        // limpa timeout da notificacao
        if (_chatNotificationTime) {
            clearTimeout(_chatNotificationTime);
            _chatNotificationTime = null;
        }

        // esconde elemento de notificao
        chatBody.find('.chat-notification').addClass('hidden');
    };

    // exibe notificacao na janela que o outro usuario esta escrevendo
    this.addNotification = function (chatRoomId, senderName) {
        // busca janela informada
        var chatBody = $('#chat-' + chatRoomId);
        // se existir janela e nao estiver fechada
        if (chatBody.length && !chatBody.hasClass('collapsed')) {
            // busca elemento de notificacao
            var notification = chatBody.find('.chat-notification')
            // seta texto no elemento e exibe
            notification.text(senderName + ' está escrevendo...').removeClass('hidden');
            // limpa timeout da notificacao
            if (_chatNotificationTime) {
                clearTimeout(_chatNotificationTime);
                _chatNotificationTime = null;
            }
            // seta timeout para esconde elemento de notificacao
            _chatNotificationTime = setTimeout(function () { notification.addClass('hidden'); }, 1200);
        }
    };

    // adiciona notificacao visual para o usuario
    this.addRoomAlert = function (chatRoomId, senderName) {
        var alertTitle = false,
            alertSound = false,
            alertMessage = false;

        if (window.window_focus) {
            if (!Painel.isDashboard()) {
                alertSound = true;
                alertMessage = true;
            }
        }
        else {
            alertTitle = true;
            alertSound = true;

            if (!Painel.isDashboard()) {
                alertMessage = true;
            }
        }

        if (alertTitle) {
            $.titleAlert(senderName + ' disse...', {
                stopOnFocus: true,
                interval: 2000
            });
        }

        if (alertSound && RPChat.config.AlertSound) {
            try{
                $('#chat-audio')[0].play();
            }
            catch(e){
                console.log('Falha ao executar audio.' + e.message);
            }
        }

        if (alertMessage && RPChat.config.AlertMessage) {
            //TODO: exibir notificacao PinesNotify
        }

        var room = $('#chat-' + chatRoomId);
        if (!room.find('textarea:first').is(":focus")) {
            room.addClass('highlight-title');
        }

        if (!Painel.isDashboard()) {
            Painel.showBadgeAlert(RPChat.totalUnreadChats());
        }
    };
};

$(function () {
    var lastWindowHeight = $(window).height();
    $(window).on("debouncedresize", function () {
        if ($(window).height() != lastWindowHeight) {
            lastWindowHeight = $(window).height();
            RPChat.updateSize();
        }
    });

    $('#btn-hide-sidebar')
        .tooltip()
        .click(function (e) {
            e.preventDefault();
            $('#btn-hide-sidebar').tooltip('hide');
            $('#sidebar #info-chats-online').text(RPChat.totalOnlineTalkers());
            $('body').addClass('hidden-bar');
            setTimeout(function () {
                $('#sidebar').click(function (e) {
                    e.preventDefault();
                    $(this).unbind('click');
                    $('body').removeClass('hidden-bar');
                    RPChat.repositionChatUI();
                });
            }, 100);
        });

    $('#content-list').slimscroll({
        disableFadeOut: true,
        railVisible: true
    });

    $('[name=status]').renderIcons({
        change: function (values) {
            RPChat.config.Status = values[0];
            RPChatHub.server.setUserConfig(RPChat.config).fail(function (e) {
                alert(e);
            });
        }
    });

    $('[name=exibir]').renderIcons({
        change: function (values) {
            RPChat.config.ShowOfflineContacts = !IsNullOrEmpty(values[0]);
            RPChatHub.server.setUserConfig(RPChat.config).fail(function (e) {
                alert(e);
            });
        }
    });

    $('[name=notificacao]').renderIcons({
        change: function (values) {
            RPChat.config.AlertSound = ($.inArray("1", values) >= 0);
            RPChat.config.AlertMessage = ($.inArray("2", values) >= 0);
            RPChatHub.server.setUserConfig(RPChat.config).fail(function (e) {
                alert(e);
            });
        }
    });
});