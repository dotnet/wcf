# Running XUnit tests cross platform

Unlike Windows, where we run tests as part of the build, we have a seperate
explicit testing step on Linux and OSX.  Over time, this special step will go
away in favor of a similar "run tests during the build" model.

`run-test.sh` is the shell script used by Jenkins in order to run all the XUnit
tests cross platform. It combines the cross platform CoreCLR, CoreFX and WCF builds
together into a test layout and then runs each test project from WCF.

In order to run tests, you need to build a bunch of different projects.  The
instructions assume you are building for Linux, but are easily modifiable for OSX.

1. Release or debug CoreCLR.  In Jenkins we use a release CoreCLR build instead
   of debug CoreCLR since it is much faster at actually running XUnit, but debug
   will work if you have the time.

   From the root of your CoreCLR enlistment on Linux, run `./build.sh Release` in
   order to build.
   
2. A corresponding version of mscorlib.dll, built on Windows but targeting
   Linux.  This can be produced by running `build.cmd linuxmscorlib Release` from
   a CoreCLR enlistment on Windows.  Remember that the runtime and mscorlib are
   tightly coupled with respect to object sizes and layout so you need to ensure
   you have either a release coreclr and release mscorlib or debug coreclr and
   debug mscorlib.
   
3. A Linux build of CoreFX.  On Windows, run `build.cmd /p:OSGroup=Linux`.  It
   is okay to build a Debug version of CoreFX and run it on top of a release
   CoreCLR (which is exactly what we do in Jenkins).
   
4. A Linux build of the native CoreFX components.  On Linux, run ./build.sh from
   src/Native in your CoreFX enlistment.
   
5. A Linux build of WCF.  On Windows, run `build.cmd /p:OSGroup=Linux`. It
   is okay to build a Debug version of WCF and run it on top of a release
   CoreCLR (which is exactly what we do in Jenkins).  


After building all the projects, we need to copy the files we built on Windows
over to our Linux machine.  The easiest way to do this is to mount a windows
share on linux or  map a Windows drive letter to a shared folder on Linux.
I followed the instructions on [howtogeek](http://www.howtogeek.com/176471/how-to-share-files-between-windows-and-linux/)
to create a shared folder on Linux ~/Desktop/Share and mapped it to a Windows drive letter.

From Windows I do this (I have my Windows enlistments under c:\git):

```
net use G: {my-Linux-IP}\Share
xcopy c:\git\wcf\bin G:\git\wcf\bin /S /Q /Y
xcopy c:\git\coreclr\bin\Product G:\git\coreclr\bin\Product /S /Q /Y
xcopy c:\git\corefx\bin G:\git\corefx\bin /S /Q /Y
```

It's significantly faster if you exclude the 'obj' and 'packages' folders.

Then on my Linux machine I do these steps to overlay from ~/Desktop/Share
into the local git bins (I have my Linux enlistments under ~/git):

```
 rsync -v -r ~/Desktop/Share/git/coreclr/bin/Product ~/git/coreclr/bin/Product
 rsync -v -r -q ~/Desktop/Share/git/corefx/bin/ ~/git/corefx/bin/
 rsync -v -r -q ~/Desktop/Share/git/wcf/bin/ ~/git/wcf/bin/ 
```

Then, run the tests.  run-test.sh defaults to wanting to use Windows tests (for
historical reasons), so we need to pass an explict path to the tests, as well as
a path to the location of CoreCLR, CoreFx, WCF and mscorlib.dll.

```
cd ~/git/wcf
./run-test.sh --coreclr-bins ~/git/coreclr/bin/Product/Linux.x64.Release \
 --mscorlib-bins ~/git/coreclr/bin/Product/Linux.x64.Release \
 --corefx-bins ~/git/corefx/bin/Linux.AnyCPU.Debug/ \
 --corefx-native-bins ~/git/corefx/bin/Linux.x64.Debug/Native \
 --wcf-bins ~/git/wcf/bin/Linux.AnyCPU.Debug \
 --wcf-tests ~/git/wcf/bin/tests/Linux.AnyCPU.Debug
```

run-test.sh should now invoke all the managed tests.
