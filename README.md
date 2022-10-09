# TextLogger
This simple C# dll is designed to save text messages with a time stamp and a unique id.
The messages are buffered and extracted into a file if there is enough time for it. The buffersize is parameterizable.

The main advantage of this class is to realize logging out of a high performance task without blocking it with file operations.
The exportation of these messages can be done in a low priority task, but this is another story, another class..., maybe your own...

# API
The API is very simple and small, there are only three main methods

## addMessage( string )
Add a message as string. After calling, the string is buffered in an internal buffer till getActEntry() is called.
Then the read pointer gets incremented.

## getActEntry()
Returns the actual entry as type of loggingBufferEntry. The struct includes the entry message itself, a timestamp and a unique id.
This method works till the entry count is zero, after that it will return null because there is no more entry to return.

## getEntryCount()
Returns the actual count of entries in the buffer. It is the difference between write and read pointer, but correctly counted also if a buffer wrap occured.

## Type TextLogger.loggingBufferEntry
The buffer is an array out of this type with the size given in the constructor.

# Author & Info
Matthias S.<br/>
matthias [at] seehauser [dot] at<br/>
https://www.seesle.at
MCI - Mechatronics
https://www.mci.at

# License
<a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/"><img alt="Creative Commons Lizenzvertrag" style="border-width:0" src="http://i.creativecommons.org/l/by-sa/4.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" href="http://purl.org/dc/dcmitype/Text" property="dct:title" rel="dct:type">KukaMatlabConnector</span> von <a xmlns:cc="http://creativecommons.org/ns#" href="http://www.github.com/seehma/KMC" property="cc:attributionName" rel="cc:attributionURL">Matthias Seehauser</a> ist lizenziert unter einer <a rel="license" href="http://creativecommons.org/licenses/by-sa/4.0/">Creative Commons Namensnennung - Weitergabe unter gleichen Bedingungen 4.0 International Lizenz</a>.<br />Beruht auf dem Werk unter <a xmlns:dct="http://purl.org/dc/terms/" href="https://www.github.com/seehma/KMC" rel="dct:source">https://www.github.com/seehma/KMC</a>.