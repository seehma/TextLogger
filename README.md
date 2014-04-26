#TextLogger
This simple C# dll is designed to save text messages with time stamp and an unique id.
It has a simple API and a parameterizable buffersize

The main advantage of this class is to realize logging out of a high performance task without blocking it with file operation or something else.
The exportation of these messages can be done in a low priority task, but this is another story, another class..., maybe your own...

#API
The API is very simple and small, there are only three main methods
####addMessage( string )
Simple API to add a string message. After calling the string is buffered in an internal buffer till getActEntry() is called.
Then the read pointer gets incremented.

####getActEntry()
Returns the actual entry as type of loggingBufferEntry. The struct includes the entry message itself, a timestamp and a unique id.
This method works till the entry count is zero, after that it will return null because there is no more entry to return.

####getEntryCount()
Returns the actual count of entries in the buffer. Mainly its the difference between write pointer and read pointer, but correctl counted over the wrap.

#### Type TextLogger.loggingBufferEntry
The buffer is an array out of this type with the size given in the constructor.

#Author & Info
Matthias S.<br/>
matthias [at] seehauser.at<br/>
http://www.seesle.at
