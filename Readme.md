# Vera Crypt mount finder commandline tool

Download binary only: [bin/Release/VeraCrypt Cmd.exe](https://github.com/BananaAcid/VeraCrypt-Cmd/blob/master/bin/Release/VeraCrypt%20Cmd.exe?raw=true) 

__Source code and binary__

    C:\VeraCrypt Cmd\bin\Release\VeraCrypt Cmd.exe
    VeraCrypt Cmd v.1.1.0.0
    Find VeraCrypt mounts and drive letters.
    VeraCrypt must be installed. Use /x for extended info, /json for json output, /csv for comma seperated values
     
    letter  Mount source (volumeName)
    K       \??\C:\Users\BananaAcid\test.vc
    M       \??\C:\Users\BananaAcid\test 2.vc


- `/x` get extended infos: letter, truecryptMode, diskLength, volumeLabel, volumeName
- `/text` is the default, generate user readable output
- `/json` sets the output generate JSON
- `/csv` sets the output to generate properly escaped CSV

_truecryptMode_: if vera crypt loaded a TrueCrypt container in compatibility mode


Version 1.1.0.0 added the output modes (code got messier). For a simpler usecase, [select version 1.0.0.0](https://github.com/BananaAcid/VeraCrypt-Cmd/tree/v1.0.0.0).


## Output for `/json /x`

    C:\VeraCrypt Cmd\bin\Release\VeraCrypt Cmd.exe /json /x
    {
            "version": "1.1.0.0",
            "mounts": [
                     
                    {
                            "letter": "K",
                            "truecryptMode": false,
                            "diskLength": 4980736,
                            "volumeLabel": "",
                            "volumeName": "\\??\\C:\\Users\\BananaAcid\\test.vc"
                    }
                    ,
                    {
                            "letter": "M",
                            "truecryptMode": false,
                            "diskLength": 4980736,
                            "volumeLabel": "",
                            "volumeName": "\\??\\C:\\Users\\BananaAcid\\test 2.vc"
                    }
            ]
    }
    
All backslashes have to be escaped. The prefix `\??\` is how windows handles paths, that can be super long.


Relates to https://github.com/BananaAcid/Selfcontained-C-Sharp-WPF-compatible-utility-classes

MIT license used.
