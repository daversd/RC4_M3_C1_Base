import torch
from torchvision import transforms, datasets
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
from torch.utils.tensorboard import SummaryWriter
import matplotlib.pyplot as plt
from PIL import Image
import os
import glob
import time
import torch.onnx

##
# Based on https://nextjournal.com/gkoehler/pytorch-mnist
##

### High-level setup ###

TRAIN = False
LOAD_LATEST = False
WRITE_LOGS = False
SAVE_CKPTS = False
EXPORT_MODEL = False

MODEL_NAME = "MNIST-Classifier"
CLASS_COUNT = 10

EPOCHS = 3              # The number of Epochs (a complete pass through the training data)
BATCH_SIZE_TRAIN = 64
BATCH_SIZE_TEST = 100

LEARNING_RATE = 0.01
MOMENTUM = 0.5

LOG_INTERVAL = 100      # Interval of steps between each saved log
CKPT_INTERVAL = -1      # Interval of epochs between each saved checkpoint

DEVICE = torch.device('cuda:0' if torch.cuda.is_available() else 'cpu')

# Define the Neural Network class, inherits from nn.Module
class Net(nn.Module):
    # The class constructor
    def __init__(self):
        super().__init__()
        self.conv1 = nn.Conv2d(1, 10, kernel_size=5)
        self.conv2 = nn.Conv2d(10, 20, kernel_size=5)
        self.conv2_drop = nn.Dropout2d()
        
        self.fc1 = nn.Linear(320, 50)           # Result of the dropout, multiplied channels!
        self.fc2 = nn.Linear(50, CLASS_COUNT)   # Map the result back to the amount of classes
    
    # The forward method, the feeds data through the network
    def forward(self, x):
        x = F.relu(F.max_pool2d(self.conv1(x), 2))
        x = F.relu(F.max_pool2d(self.conv2_drop(self.conv2(x)), 2)) # ouputs size([1, 20, 4, 4])
        x = x.view(-1, 320)
        x = F.relu(self.fc1(x))
        x = F.dropout(x, training=self.training)
        x = self.fc2(x)
        
        return F.log_softmax(x, dim=1)          # Return the highest probability


##
# Functions
##



##
# Execution
##