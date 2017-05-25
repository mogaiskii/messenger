import socket


class ClientObject:
    __NAME_LEN = 32
    __SERVER = "SERVER"

    __PARENT = None

    username = ""

    on_recived = None

    # username
    # client
    # #TODO: events

    def __init__(self, connection, parent): #parent: Server
        self.client = connection
        self.__PARENT = parent

    def __del__(self):
        self.client.close()

    def process(self):
        self.recive()

    def recive(self):
        try:
            self.client.setblocking(0)
            while True:

                data = b""
                # reciving data
                try:
                    while True:
                        recived = self.client.recv(64)
                        if not recived:
                            break
                        data += recived
                except:
                    if not data:
                        continue

                message = data.decode("unicode_internal")

                send_to = message[:self.__NAME_LEN]
                send_to = send_to[:send_to.find("%")]
                if send_to==self.__SERVER:
                    command = message[self.__NAME_LEN:self.__NAME_LEN+4]
                    if command=="AUTH":
                        self.__PARENT.log("Auth request")
                        self.username = message[self.__NAME_LEN+4:]
                        password = self.username[self.username.rfind("%")+1:]
                        self.username = self.username[:self.username.find("%")]

                        if not self.__PARENT.add_client(self.username, password, self.send):
                            raise Exception(self.username+" Denied")

                    elif command=="ADD_":
                        name_to = message[self.__NAME_LEN+4:]
                        self.__PARENT.add_request(self.username,name_to)

                    elif command=="EXIT":
                        raise Exception(self.username + " Disconnected")

                    elif command=="REG_":
                        t_username = message[self.__NAME_LEN+4:2*self.__NAME_LEN+4]
                        t_password = message[2*self.__NAME_LEN+4:]

                        if self.__PARENT.registration_request(t_username,t_password):
                            self.send(self.__SERVER, t_username, "REG_FINE")
                            raise Exception(self.username +" Registred")

                        else:
                            self.send(self.__SERVER, t_username, "REG_BAD")
                            raise Exception(self.username + "NOT Registred")

                elif self.username:
                    sent_from = self.username
                    send_to = send_to
                    message = message[self.__NAME_LEN:]
                    self.on_recived(sent_from, send_to, message)

        except Exception as ex:
            self.client.close()
            self.__PARENT.log(ex)
            return

        finally:
            if self.username:
                self.__PARENT.remove_client(self.username)

            self.client.close()
            return

    def send(self, sent_from, send_to, message):
        if self.username or sent_from==self.__SERVER:
            if sent_from!=self.__SERVER:
                data = (sent_from+"^"+send_to+":"+message).encode("unicode_internal")
            else:
                data = (sent_from + message).encode("unicode_internal")
            if len(data) < 64:
                while len(data) < 64:
                    data += b'\x00'
            self.client.send(data)

