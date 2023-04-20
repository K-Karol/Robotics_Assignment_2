from _thread import *
import threading
class PayloadQueue:
    '''This class is a basic implementation of a concurrent queue (FIFO) which allows to push new payloads to the queue and pop new payloads from the queue.'''
    items = []
    count = 0
    lock = None
    def __init__(self):
        self.lock = allocate_lock()
    def push(self, payload):
        """Pushes a payload into the queue"""
        self.lock.acquire()
        self.items.append(payload)
        self.count += 1
        self.lock.release()
    def pop(self):
        """Removes and returns the first Payload, or None if there are no payloads available."""
        self.lock.acquire()
        if(self.count > 0):
            self.count -= 1
            item = self.items.pop(0)
            self.lock.release()
            return item
        self.lock.release()
        return None