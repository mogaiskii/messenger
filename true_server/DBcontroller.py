import pymysql
import threading


class DBcontroller:
    """Handle the connection with database"""
    __LOCK = threading.RLock()
    def __init__(self, d_base: str, host: str, user_id: str, password: str):
        # connect
        self.connection = pymysql.connect(host=host, user=user_id,
                                          passwd=password, db=d_base)

    def __del__(self):
        # disconnect
        #self.connection.close()
        pass

    def check_auth(self, username, password_hash):
        # checks the authorisation

        self.__LOCK.acquire() # blocks the thread
        
        cur = self.connection.cursor()

        
        if not cur.execute("SELECT password FROM users WHERE username='"+username+"'"):
            cur.close()
            self.__LOCK.release()
            return False
        
        data = cur.fetchone()[0]
        cur.close()
        
        self.__LOCK.release() # unlocks the thread
        
        return data == password_hash

    def try_register(self, username:str, password_hash:str) -> bool:
        # tries to register new user
        # -> True  if everything OK, False otherwise
        result = True

        self.__LOCK.acquire()

        cur = self.connection.cursor() # Checks if username is free
        verify_request = "SELECT username FROM users WHERE username='"+username+"'"
        if cur.execute(verify_request):
            result = False
        cur.close()

        if result:
            cur = self.connection.cursor() # Registrate the user
            sql = "INSERT INTO `users`(`username`, `password`) VALUES (%s, %s)"
            cur.execute(sql,(username,password_hash))
            cur.close()
            self.connection.commit()

        self.__LOCK.release()

        return result
