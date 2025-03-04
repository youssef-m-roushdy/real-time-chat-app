import { inject, Injectable, signal } from '@angular/core';
import { User } from '../models/user';
import { AuthService } from './auth.service';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { Message } from '../models/message';
@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private authService = inject(AuthService);
  private hubUrl = 'http://localhost:5000/hubs/chat';
  onlineUsers = signal<User[]>([]);
  currentOpenChat = signal<User | null>(null);
  chatMessages = signal<Message[]>([]);
  isLoading = signal<boolean>(true);

  private hubConnection?: HubConnection;

  startConnection(token: string, senderId?: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}?senderId=${senderId || ''}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch((err) => {
        console.log('Connection or login error' + err);
      });

    this.hubConnection.on('Notify', (user: User) => {
      Notification.requestPermission().then((result) => {
        if (result === 'granted') {
          new Notification('Active now 🟠', {
            body: user.fullName + 'is online now',
            icon: user.profileImage,
          });
        }
      });
    });

    this.hubConnection!.on('OnlineUsers', (user: User[]) => {
      console.log(user);
      this.onlineUsers.update(() =>
        user.filter(
          (user) =>
            user.userName !== this.authService.currentLoggedUsser!.userName
        )
      );
    });

    this.hubConnection!.on('NotifyTypingToUser', (senderUserName) => {
      this.onlineUsers.update((users) =>
        users.map((user) => {
          console.log(user.userName)
          if (user.userName === senderUserName.toString()) {
            // Fix: Compare user.userName
            user.isTyping = true;
          }
          return user;
        })
      );

      setTimeout(() => {
        this.onlineUsers.update((users) =>
          users.map((user) => {
            if (user.userName === senderUserName) {
              user.isTyping = false;
            }
            return user;
          })
        );
      }, 2000);
    });

    this.hubConnection!.on('ReceiveMessageList', (message) => {
      this.chatMessages.update((messages) => [...message, ...messages]);
      this.isLoading.update(() => false);
    });

    this.hubConnection!.on('ReceiveNewMessage', (message: Message) => {
      document.title = '(1) New Message';
      this.chatMessages.update((messages) => [...messages, message]);
    });
  }

  disConnectConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((err) => console.log(err));
    }
  }

  status(userName: string) {
    const currentChatUser = this.currentOpenChat();
    if (!currentChatUser) {
      return 'ofline';
    }

    const onlineUser = this.onlineUsers().find(
      (user) => user.userName === userName
    );

    return onlineUser?.isTyping ? 'Typing...' : this.isUserOnline();
  }

  isUserOnline() {
    let onlineUser = this.onlineUsers().find(
      (user) => user.userName === this.currentOpenChat()?.userName
    );
    return onlineUser?.isOnline ? 'online' : this.currentOpenChat()!.userName;
  }

  loadMessages(pageNumber: number) {
    this.chatMessages.set([]);
    this.hubConnection
      ?.invoke('LoadMessages', this.currentOpenChat()?.id, pageNumber)
      .then()
      .catch()
      .finally(() => {
        this.isLoading.update(() => false);
      });
  }

  sendMessage(message: string) {
    this.chatMessages.update((messages) => [
      ...messages,
      {
        content: message,
        senderId: this.authService.currentLoggedUsser!.id,
        receiverId: this.currentOpenChat()?.id!,
        createdAt: new Date().toString(),
        isRead: false,
        id: 0,
      },
    ]);

    this.hubConnection
      ?.invoke('SendMessage', {
        receiverId: this.currentOpenChat()?.id,
        content: message,
      })
      .then((id) => {
        console.log('message send to', id);
      })
      .catch((error) => {
        console.log(error);
      });
  }

  notifyTyping() {
    this.hubConnection
      ?.invoke('NotifyTyping', this.currentOpenChat()?.userName)
      .then((x) => {
        console.log('Notify for ' + x);
      })
      .catch((error) => {
        console.log(error);
      });
  }
}
