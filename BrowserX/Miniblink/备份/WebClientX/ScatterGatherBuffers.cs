using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBlink.WebClientX
{
    internal class ScatterGatherBuffers
    {
        private class MemoryChunk
        {
            internal byte[] Buffer;

            internal int FreeOffset;

            internal MemoryChunk Next;

            internal MemoryChunk(int bufferSize)
            {
                Buffer = new byte[bufferSize];
            }
        }

        private MemoryChunk headChunk;

        private MemoryChunk currentChunk;

        private int nextChunkLength = 1024;

        private int totalLength;

        private int chunkCount;

        private bool Empty
        {
            get
            {
                if (headChunk != null)
                {
                    return chunkCount == 0;
                }
                return true;
            }
        }

        internal int Length => totalLength;

        internal ScatterGatherBuffers()
        {
        }

        internal ScatterGatherBuffers(long totalSize)
        {
            if (totalSize > 0)
            {
                currentChunk = AllocateMemoryChunk((int)((totalSize > int.MaxValue) ? int.MaxValue : totalSize));
            }
        }

        internal BufferOffsetSize[] GetBuffers()
        {
            if (Empty)
            {
                return null;
            }
            BufferOffsetSize[] array = new BufferOffsetSize[chunkCount];
            int num = 0;
            for (MemoryChunk next = headChunk; next != null; next = next.Next)
            {
                array[num] = new BufferOffsetSize(next.Buffer, 0, next.FreeOffset, copyBuffer: false);
                num++;
            }
            return array;
        }

        internal void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int num = (!Empty) ? (currentChunk.Buffer.Length - currentChunk.FreeOffset) : 0;
                if (num == 0)
                {
                    MemoryChunk next = AllocateMemoryChunk(count);
                    if (currentChunk != null)
                    {
                        currentChunk.Next = next;
                    }
                    currentChunk = next;
                }
                int num2 = (count < num) ? count : num;
                Buffer.BlockCopy(buffer, offset, currentChunk.Buffer, currentChunk.FreeOffset, num2);
                offset += num2;
                count -= num2;
                totalLength += num2;
                currentChunk.FreeOffset += num2;
            }
        }

        private MemoryChunk AllocateMemoryChunk(int newSize)
        {
            if (newSize > nextChunkLength)
            {
                nextChunkLength = newSize;
            }
            MemoryChunk result = new MemoryChunk(nextChunkLength);
            if (Empty)
            {
                headChunk = result;
            }
            nextChunkLength *= 2;
            chunkCount++;
            return result;
        }
    }

}
