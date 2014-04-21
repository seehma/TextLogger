using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KukaMatlabConnector
{
    public class TextLogger
    {
        const uint internalbufferSize_ = 255;
        uint bufferSize_;
        uint readPointer_;
        uint writePointer_;
        System.Threading.Mutex mutexEntryCount;

        public uint WritePointer
        {
            get
            {
                return writePointer_;
            }
            set
            {
                writePointer_ = value;
            }
        }

        public uint ReadPointer
        {
            get
            {
                return readPointer_;
            }
            set
            {
                if (value == 255)
                {
                    ;
                }
                readPointer_ = value;
            }
        }

        int id_;
        loggingBufferEntry[] loggingBuffer_;

        public struct loggingBufferEntry
        {
            public int id;
            public System.DateTime timeStamp;
            public System.String message;

            public loggingBufferEntry(int extID, System.DateTime extTimeStamp, System.String extMessage)
            {
                id = extID;
                timeStamp = extTimeStamp;
                message = extMessage;
            }
        }



        /* ====================================================================================================================== */
        /** 
         *   CLASS TextLogger
         * 
         * 
         * 
         */
        /* ====================================================================================================================== */
        public TextLogger( uint bufferSize )
        {
            mutexEntryCount = new System.Threading.Mutex(false, "entryCount");

            if (bufferSize != 0)
            {
                loggingBuffer_ = new loggingBufferEntry[bufferSize];
                bufferSize_ = bufferSize;
            }
            else
            {
                loggingBuffer_ = new loggingBufferEntry[internalbufferSize_];
                bufferSize_ = internalbufferSize_;
            }

            readPointer_ = 0;
            writePointer_ = 0;
            id_ = 0;
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         * 
         * 
         * 
         */
        /* ---------------------------------------------------------------------------------------------------------------------- */
        public bool addMessage(String message)
        {
            bool errReturn = false;
            uint localWritePointer = 0;
            System.DateTime dateTime;
            loggingBufferEntry entry;

            // save the write pointer to write a message on that line
            localWritePointer = WritePointer;

            // increment the write pointer and if valid enter the message to the old place
            if (!incrementWritePointer())
            {
                id_++;
                dateTime = new System.DateTime();
                entry = new loggingBufferEntry(id_, dateTime, message);

                dateTime.AddTicks(System.DateTime.Now.Ticks);

                loggingBuffer_[localWritePointer] = entry;
            }

            return errReturn;
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         * 
         * 
         * 
         */
        /* ---------------------------------------------------------------------------------------------------------------------- */
        public uint getEntryCount()
        {
            uint localEntryCount = 0;
            int localPointerDiff = 0;

            mutexEntryCount.WaitOne();

            localPointerDiff = Convert.ToInt32(WritePointer) - Convert.ToInt32(ReadPointer);

            // check if the pointer difference is positive or negative
            if (localPointerDiff > 0)
            {
                localEntryCount = WritePointer - ReadPointer;
            }
            else if (localPointerDiff < 0)
            {
                localEntryCount = (bufferSize_ - ReadPointer) + WritePointer;
            }
            else
            {
                localEntryCount = 0;
            }

            mutexEntryCount.ReleaseMutex();

            return localEntryCount;
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         * 
         * 
         * 
         */
        /* ---------------------------------------------------------------------------------------------------------------------- */
        private void incrementReadPointer()
        {
            mutexEntryCount.WaitOne();

            ReadPointer++;
            if (ReadPointer > (bufferSize_ - 1))
            {
                ReadPointer = 0;
            }

            mutexEntryCount.ReleaseMutex();
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         * 
         * 
         * 
         */
        /* ---------------------------------------------------------------------------------------------------------------------- */
        private bool incrementWritePointer()
        {
            bool errReturn = false;
            uint localWritePointer;

            mutexEntryCount.WaitOne();

            localWritePointer = WritePointer;

            WritePointer++;
            if (WritePointer > (bufferSize_ - 1))
            {
                WritePointer = 0;
            }

            // writepointer can not be equal to readpointer after incrementation
            if (WritePointer == ReadPointer)
            {
                WritePointer = localWritePointer;
                errReturn = true;
            }

            mutexEntryCount.ReleaseMutex();

            return errReturn;
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         * 
         * 
         * 
         */
        /* ---------------------------------------------------------------------------------------------------------------------- */
        public TextLogger.loggingBufferEntry getActEntry()
        {
            TextLogger.loggingBufferEntry entry = new TextLogger.loggingBufferEntry();

            if (getEntryCount() > 0)
            {
                entry = loggingBuffer_[ReadPointer];
                incrementReadPointer();
            }

            return entry;
        }
    }
}