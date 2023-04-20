import socket
import time
import errno
from _thread import *
import threading
from payload import Payload
from payload_queue import PayloadQueue
#https://stackoverflow.com/questions/16745409/what-does-pythons-socket-recv-return-for-non-blocking-sockets-if-no-data-is-r

class SocketConnection:
    '''This class handles the socket connection, allowing to start a connection, wait for the client to connect, and both writing and reading from the socket connection.'''
    port = 5000
    socket = None
    client = None
    stop = False
    payloads = None
    readerThread = None
    def __init__(self, port:int = 5000):
         self.port = port
         self.payloads = PayloadQueue()
    def start(self):
        """Starts the socket connection and waits for the client to connect"""
        addr = socket.getaddrinfo('0.0.0.0', self.port)[0][-1]
        self.socket = socket.socket()
        self.socket.bind(addr)
        self.socket.listen(1)
        self.socket.setblocking(False)
        while self.client == None:
            try:
                self.client, client_address = self.socket.accept()
                self.readerThread = threading.Thread(target=self.reader)
                self.readerThread.start()
            except socket.error as e:
                err = e.args[0]
                if err == errno.EAGAIN or err == errno.EWOULDBLOCK:
                    time.sleep(1)
                    continue
                else:
                    raise e
                
    def close(self):
        """Stops the reader thread and closes the socket connection"""
        self.stop = True
        self.socket.close()
    def reader(self):
        """This function is ran on a seperate thread that reads from the socket connection, parses the data as a Payload and pushes the payload to the queue"""
        print("Starting read")
        while not self.stop:
            data = None
            try:
                data = self.client.recv(1024)
            except timeout as e:
                err = e.args[0]
                if err == 'timed out':
                    time.sleep(1)
                    continue
                else:
                    print(e)
            except error as e:
                 print(e)
            if not data:
                continue

            payload = Payload.Parse(data.decode("utf-8"))
            if payload == None:
                continue

            self.payloads.push(payload)

            
    def dispatch(self,payload):
        self.client.send(payload.ToString())