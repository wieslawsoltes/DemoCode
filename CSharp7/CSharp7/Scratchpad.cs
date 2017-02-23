// Copyright 2017 Jon Skeet. All rights reserved. Use of this source code is governed by the Apache License 2.0, as found in the LICENSE.txt file.
namespace CSharp7
{
    class Scratchpad
    {
        int x;

        public void foo(int x)
        {
            this.x = x;
            this.foo(10);
        }

        static void Main()
        {
        }
    }
}
