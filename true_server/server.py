import socket
import threading

from true_server.client_object import ClientObject
from true_server.DBcontroller import DBcontroller


class Server:
    __port = 8010
    __clients = {}
    __listener = socket.socket()

    __DBcontr = DBcontroller("main", "localhost", "root", "")

    __LOCK = threading.RLock()

    def __init__(self):
        try:
            self.__listener.bind(('', self.__port))

            print("Alive")
            print("CTRL+C to exit")

            while True:
                self.__listener.listen(1)
                client, addr = self.__listener.accept()
                client_object = ClientObject(client,self)
                # TODO: client_object events
                client_object.on_recived = self.message_recived

                _thread = threading.Thread(target=client_object.process)
                _thread.start()
        except KeyboardInterrupt:
            # clear everything
            exit(0)
        except:
            raise

    def message_recived(self, sent_from, send_to, message):
        """sent_form: str, send_to: str, message: str   -> None"""
        if send_to in self.__clients.keys():
            self.__clients[sent_from](sent_from, send_to, message)
            self.__clients[send_to](sent_from, send_to, message)
        else:
            self.__clients[sent_from](sent_from, send_to, "Вне сети")

    def add_request(self, name_from, name_to):
        """name_from: str, name_to: str   -> None"""
        if name_to in self.__clients.keys():
            self.__clients[name_to]("SERVER", name_to, "ADD_"+name_from)
            self.__clients[name_to](name_from, name_to, name_from+" добавил вас в друзья")
            #self.__clients[name_from]("SERVER", name_from, "ADDD"+name_to) ADDD - ADDeD
        else:
            self.__clients[name_from]("SERVER", name_from, "NADD"+name_to)

    def registration_request(self, username, password_hash):
        """username: str, password_hash: str    -> bool"""
        if "%" in username:
            username = username[:username.find("%")]
        return self.__DBcontr.try_register(username, password_hash)

    def add_client(self, username, password_hash, send_func):
        """username: str, password_hash: str    -> bool"""
        if not self.__DBcontr.check_auth(username, password_hash):
            send_func("SERVER", username, "EXIT")
            return False

        self.__LOCK.acquire()
        if not username in self.__clients.keys():
            self.__clients[username] = send_func
            print(username+" Connected")
        else:
            self.remove_client(username)
            self.add_client(username,password_hash,send_func)
        self.__LOCK.release()

        return True

    def remove_client(self, username):
        """username: str    -> None"""
        if username in self.__clients.keys():
            self.__clients.pop(username)

if __name__=="__main__":
    # serv = Server()
    Server()