import FileDisplayer from "~/components/FileItem/FileDisplayer";
import type { LoaderFunctionArgs } from "react-router";
import { useState } from "react";
import type { TreeItem } from "~/components/FileTree/FileTree";
import type { fileItem } from "~/components/FileItem/FileItem";




export default function UserDashboard({ files }: { files: fileItem[]}) {
  return <FileDisplayer files={files} />;
}
