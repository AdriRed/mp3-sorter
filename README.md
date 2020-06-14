# Mp3 Sorter

## How it works?

This .exe reads the mp3's metadata and copies the files to a new folder first grouping by Artist, then by Album ([year] Album) and then copies all .mp3 of the album with the format (00. (song) - (album artist).mp3).

## Example

In a folder (C:\Temp) we have 7 songs:

|Filename|Album|Year|Artist|Song|Number|
|---|---|---|---|---|---|
|sadf.mp3|Discovery|2001|Daft Punk|One More Time|1|
|harder better faster stronger.mp3|Discovery|2001|Daft Punk|Harder, Better, Faster, Stronger|4|
|get lucky.mp3|Random Access Memories|2013|Daft Punk|Get Lucky|8|
|rehash.mp3|Gorillaz|2001|Gorillaz|Re-Hash|1|
|54.mp3|Gorillaz|2001|Gorillaz|5/4|2|
|clint-eastwood-gorillazzz.mp3|Gorillaz|2001|Gorillaz|Clint Eastwood|5|
|plastic-beach-welcome.mp3|Plastic Beach|2010|Gorillaz|Welcome To The World Of The Plastic Beach|2|
|melancolyill.mp3|Plastic Beach|2010|Gorillaz|On Melancholy Hill|9|

After giving the source folder (C:\Temp) and an output folder (C:\Music), will show you what will copy to the new folder and if your agreed, will copy all the files in this structure:

```
\Daft Punk\[2001] Discovery\01. One More Time - Daft Punk.mp3  
\Daft Punk\[2001] Discovery\04. Harder, Better, Faster, Stronger - Daft Punk.mp3  
\Daft Punk\[2013] Random Access Memories\08. Get Lucky - Daft Punk.mp3  
\Gorillaz\[2001] Gorillaz\01. Re-Hash - Gorillaz.mp3  
\Gorillaz\[2001] Gorillaz\02. 54 - Gorillaz.mp3 -- (/ not allowed)  
\Gorillaz\[2001] Gorillaz\05. Clint Eastwood - Gorillaz.mp3  
\Gorillaz\[2001] Plastic Beach\02. Welcome To The World Of The Plastic Beach - Gorillaz.mp3  
\Gorillaz\[2001] Plastic Beach\09. On Melancholy Hill - Gorillaz.mp3 
```

## Dependencies

* [TagLibSharp 2.2.0](https://www.nuget.org/packages/TagLibSharp)
