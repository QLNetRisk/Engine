MainFrame
=====

MainFrame is a financial library written in C# for the Windows environment. The library is a software suite comprised by data clients, a quantitative finance library and front-end solutions. The financial library extends the open-source reference library QuantLib written in C++ and its C# counterpart QLNet. The front-end solutions provide means to expose data and functionality provided by the library to end-users and allows for distribution of integrated software components.

## Development workflow

###### MainFrame uses git source control.

To contribute features, you should clone the repository and setup the required dependencies:

```
git clone "G:\EXCEL\MIDDLEOFFICE\Risikostyring\Code share\mainframe.git"
cd mainframe
git submodule init
git submodule foreach -q --recursive 'git checkout $(git config -f $toplevel/.gitmodules submodule.$name.branch || echo master)'
git submodule update
```

In order to get a working copy, fetch the connections.config file from the Code Share directory and place it into the mainframe root directory. If you need to use MDB connections, insert your Sandsli credentials in the MDB connection string inside the connections.config file. Finally, compile the solutions in the following order:

***Right click solution file in Visual Studio -> Restore NuGet Packages -> Build solution (Ctrl-Shift-B)***

1. QLNet
2. QLib
3. DataLayer
4. ExcelAPI
5. QFront
