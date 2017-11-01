# MTMerge - MessageTable Merge Tool

A tool that merges two or more [MESSAGETABLE](https://msdn.microsoft.com/en-us/library/windows/desktop/aa381027(v=vs.85).aspx) resource files.

## What's The Problem?

It seem that [there is no way](https://stackoverflow.com/questions/47051529/more-than-one-messagetable-resource-in-the-same-binary) to have more than one MESSAGETABLE resource.

## Solution

A build step between message compiler and resource compiler to merge several MESSAGETABLE resource files.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
