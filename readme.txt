The PhotoProperties library is used to analyze the tag properties of 
an image. Encapsulated metadata within the image is decrypted by the 
System.Drawing.Imaging.PropertyItem methods and formatted with the
library's stored metadata. The formatted data can be accessed 
individually, or as a single XML file or stream.

To store the PhotoMetadata.xml file as a resource, I've included the
FileResGen.exe console application from Paul DiLascia's MOTLib.NET 
"goodies."  The CreateRes.bat file can be used to run FileResGen.

