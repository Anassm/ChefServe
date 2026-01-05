import { createContext, useContext } from "react";
import type { fileItem } from "~/components/FileItem/FileItem";



export const selectedFileContext = createContext<{
  selectedFile: fileItem | null;
  setSelectedFile: (file: fileItem | null) => void;
} | null>(null);


export type RefreshSidebarContextType = {
  refresh: boolean;
  setRefresh: React.Dispatch<React.SetStateAction<boolean>>;
};

export const refreshSidebarContext = createContext<RefreshSidebarContextType | null>(null);

