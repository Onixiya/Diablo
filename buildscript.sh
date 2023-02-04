mv Bundles/diablo.bundle Bundles/diablo.modhelpernoload
mv Bundles/diabloclips.bundle Bundles/diabloclips.modhelpernoload
rm -r bin
rm -r obj
dotnet build
cp bin/Debug/net6.0/Diablo.dll /home/silentstorm/.steam/steam/steamapps/common/BloonsTD6/Mods