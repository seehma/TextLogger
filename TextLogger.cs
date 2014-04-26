using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

/* ====================================================================================================================== */
/**
 *  Namespace TextLogger
 * 
 */
/* ====================================================================================================================== */
namespace TextLogger
{
    /* ------------------------------------------------------------------------------------------------------------------------------- */
    /**
     *  Class TextLogger 
     */
    /* ------------------------------------------------------------------------------------------------------------------------------- */
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

        // counts the id for every instance, starts with 1 and goes till the instance is closed
        int id_;

        // empty logging buffer, gets initialized when the constructor runs through
        loggingBufferEntry[] loggingBuffer_;

        // loggingBufferEntry type ... the buffer contains an array of this type
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
         *   constructor ... of Class TextLogger
         *   
         *   @param    bufferSize ... Size of Buffer in entries
         */
        /* ====================================================================================================================== */
        public TextLogger( uint bufferSize )
        {
            // initialize mutex for TextLogger Class
            mutexEntryCount = new System.Threading.Mutex(false, "entryCount");

            // check if buffersize is non equal to zero, then initialize buffer with given size
            // else take the default size of 255
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

            readPointer_ = 0; // initialize readPointer with zero
            writePointer_ = 0; // initialize writePointer with zero
            id_ = 1; // initialize the id variable with 1 => zero would be a non valid id
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         *  @brief   inserts a message into the buffer, if the buffer is full no more message can be entered so it returns 
         *           true => error happens
         * 
         *  @param   message ... message as String type
         *  
         *  @retval  bool ... returns fals if the message was entered, returns true if an error happens
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
            else
            {
                errReturn = true;
            }

            return errReturn;
        }

        /* ---------------------------------------------------------------------------------------------------------------------- */
        /**
         *  @brief   public function which returns the actual count of entries in the buffer
         *           if not equal to zero getActEntry will return one entry out of the buffer
         * 
         *  @retval  uint ... actual count of entries in the buffer
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
         *  @brief   internal function which increments the read pointer and looks if it is allowed to increment
         * 
         *  @retval  none
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
         *  @brief   internal function to increment the buffers write pointer
         * 
         *  @retval  none
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
         *  @brief    returns the entry on which the read pointer stands in buffer
         * 
         *  @retval   TextLogger.loggingBufferEntry ... entry as described in the structure declaration
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