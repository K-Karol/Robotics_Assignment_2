from _thread import *
import threading
class PayloadQueue:
    items = []
    count = 0
    lock = None
    def __init__(self):
        self.lock = allocate_lock()
    def push(self, payload):
        self.lock.acquire()
        self.items.append(payload)
        self.count += 1
        self.lock.release()
    def pop(self):
        self.lock.acquire()
        if(self.count > 0):
            self.count -= 1
            item = self.items.pop(0)
            self.lock.release()
            return item
        self.lock.release()
        return None