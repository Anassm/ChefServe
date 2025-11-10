import FileDisplayer from "~/components/FileItem/FileDisplayer";
import type { LoaderFunctionArgs } from "react-router";
import { useState } from "react";
import type { TreeItem } from "~/components/FileTree/FileTree";




export default function UserDashboard({ files }: { files: fileItem[]}) {
  return <FileDisplayer files={files} />;
}
