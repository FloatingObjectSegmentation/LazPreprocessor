Usage:

- Open in Visual Studio 2017
- **$config = project common/GConfig.cs**

### Download

- In $config, under **[downloader]** section, set the execution type
    - ```TypeOfExecution.Single``` means 




# OLD


# Laz Preprocessor

Laz preprocessor is used to transform .laz files into other file forms and also extract additional
data if needed. It can:

- Add RGB values for points in ARSO datasets (Using Slemenik's application)
- Output xyzRGB text files
- Output class text files - each line is a number which determines a point's class
- Output floating object class files - using the RBNN algorithm

## Dependencies

- You need to have the tool ```las2txt``` in your path. You can check the LASTools project to get it.

## How it works

#### Transform a set of .laz files to all the above stated outputs

- To acquire the initial data you have two options:
    - Use Slemenik's tool to download RGB enriched .laz files
    - Have a set of .laz files in a directory prepared
- Configure the ```core``` project via the ```CoreProcedure.cs``` file's variables in the config region
    - set the path to your ```rbnn.exe``` file (it is included in the core project in the resources folder)
    - set the desired radius values you would like rbnn to execute for
- Configure the ```executor``` project via the ```Program.cs``` file's variables
    - set the directory in which the .laz files you want to preprocess are contained
- Run the ```executor``` project and wait for the program to finish
- You will now have all your desired files residing by the .laz files in the same folder
