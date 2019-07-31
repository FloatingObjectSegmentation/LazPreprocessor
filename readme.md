README UPDATED ON COMMIT ()


Usage:

- Open in Visual Studio 2017
- **$config = project common/GConfig.cs**

### Define your workspace

- Create a folder with 4 subfolders: lidar, dmr, tests and augmentation.
- in $config, change the workspace dirs variables in region **[global]**
- in $config, change the tool paths to point to your resources folder

### Download

**Result: las files and dmr files in /dmr, /lidar directories of your workspace**

- In $config, under **[downloader]** section, set the execution type
    - They are self explanatory, define the elements you want processed bellow in the downloader section.
- Rebuild project, set downloader as startup project and start the program.


### Preprocess

**Result: files able to be viewed/labeled in PointCloudia, create augmentables from. 4 files. saved in workspace/lidar/:**
- .txt point cloud file
- .txt class file (class labels for each point)
- .txt intensity file (intensity labels for each point)
- .pcd RBNN result file

- In $config, set the **RBNN_R_VALUES** parameter to include all the RBNN values you want to obtain.
- Set the **executor** project as startup project and run it.

### Sampling of augmentables
**Result: files filled with augmentables in workspace/augmentation**

- In $config set the following:
    - **candidateContextRadius** - a candidate has a context of some radius. This is important because it will determine the upper Z bound of sampling augmentables. (We don't want to sample 500 meters above the highest point of the terrain because the context will always be empty and the example, therefore, trivial)
    - **ObjectsToAdd** - how many objects do you want to create
    - **AUGMENTATION_RBNN_R_SPAN** - for determining the distance between the augmentable and the closest point in the dataset. Should be between a small **r** and the **candidateContextRadius**. The denser you make it, the more exact it will be, but the longer it will take to compute.
    - **GetAugmentableObjectPalette()** - definitions for possible floating objects that will be randomly sampled. 

- Set **augmentation_sampler** project as startup project and run the program.

#### Augmentation file format

for each augmentable:

- [int id] [X,Y,Z vec float position] [X,Y,Z vec float scale] [string shape name] [X,Y,Z vec float airplane position] [float distance from transitive ground]\n

## EXTRA

Viewing data in PointCloudia

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
