#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

EF_VER=6.1.1
EF_DIR=$DIR/../src/packages/EntityFramework.$EF_VER

OUTPUT_DIR="$DIR/../src/Pablo.Gallery/bin"
CONTEXT_DLL=Pablo.Gallery.dll

if [ ! -f $OUTPUT_DIR/$CONTEXT_DLL ]; then
	echo "You must build the project first before running this script"
	exit 1
fi

echo "Copying migrate.exe and entity framework dll's to a temp directory"
TMP_DIR=`mktemp -d -t pablo.gallery.migrate`

cp -R $EF_DIR/tools/* $TMP_DIR
cp -R $EF_DIR/lib/net40/* $TMP_DIR

echo "Updating database..."
cd $OUTPUT_DIR
mono $TMP_DIR/migrate.exe $CONTEXT_DLL /startupDirectory=$OUTPUT_DIR /startupConfigurationFile=../Web.config $@

echo "Cleaning up..."
rm -Rf $TMP_DIR

echo "Done!"