using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenDentBusiness;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>Handles documents and images for the Images module</summary>
	public class Documents {

		///<summary></summary>
		public static Document[] GetAllWithPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Document[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM document WHERE PatNum="+POut.Long(patNum)+" ORDER BY DateCreated";
			DataTable table=Db.GetTable(command);
			return Crud.DocumentCrud.TableToList(table).ToArray();
		}

		///<summary>Gets the document with the specified document number.</summary>
		public static Document GetByNum(long docNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),docNum);
			}
			Document doc=Crud.DocumentCrud.SelectOne(docNum);
			if(doc==null){
				return new Document();
			}
			return doc;
		}

		public static Document[] Fill(DataTable table){
			//No need to check RemotingRole; no call to db.
			if(table==null){
				return new Document[0];
			}
			List<Document> list=Crud.DocumentCrud.TableToList(table);
			return list.ToArray();
		}

		/*
		private static Document[] RefreshAndFill(string command) {
			//No need to check RemotingRole; no call to db.
			return Fill(Db.GetTable(command));
		}*/

		///<summary>Usually, set just the extension before passing in the doc.  Inserts a new document into db, creates a filename based on Cur.DocNum, and then updates the db with this filename.  Should always refresh the document after calling this method in order to get the correct filename for RemotingRole.ClientWeb.</summary>
		public static long Insert(Document doc,Patient pat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				doc.DocNum=Meth.GetLong(MethodBase.GetCurrentMethod(),doc,pat);
				return doc.DocNum;
			}
			doc.DocNum=Crud.DocumentCrud.Insert(doc);
			//If the current filename is just an extension, then assign it a unique name.
			if(doc.FileName==Path.GetExtension(doc.FileName)) {
				string extension=doc.FileName;
				doc.FileName="";
				string s=pat.LName+pat.FName;
				for(int i=0;i<s.Length;i++) {
					if(Char.IsLetter(s,i)) {
						doc.FileName+=s.Substring(i,1);
					}
				}
				doc.FileName+=doc.DocNum.ToString()+extension;//ensures unique name
				//there is still a slight chance that someone manually added a file with this name, so quick fix:
				string command="SELECT FileName FROM document WHERE PatNum="+POut.Long(doc.PatNum);
				DataTable table=Db.GetTable(command);
				string[] usedNames=new string[table.Rows.Count];
				for(int i=0;i<table.Rows.Count;i++) {
					usedNames[i]=PIn.String(table.Rows[i][0].ToString());
				}
				while(IsFileNameInList(doc.FileName,usedNames)) {
					doc.FileName="x"+doc.FileName;
				}
				/*Document[] docList=GetAllWithPat(doc.PatNum);
				while(IsFileNameInList(doc.FileName,docList)) {
					doc.FileName="x"+doc.FileName;
				}*/
				Update(doc);
			}
			return doc.DocNum;
		}

		///<summary></summary>
		public static void Update(Document doc){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),doc);
				return;
			}
			Crud.DocumentCrud.Update(doc);
		}

		///<summary></summary>
		public static void Delete(Document doc){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),doc);
				return;
			}
			string command= "DELETE from document WHERE DocNum = '"+doc.DocNum.ToString()+"'";
			Db.NonQ(command);
			DeletedObjects.SetDeleted(DeletedObjectType.Document,doc.DocNum);
		}

		///<summary>This is used by FormImageViewer to get a list of paths based on supplied list of DocNums. The reason is that later we will allow sharing of documents, so the paths may not be in the current patient folder.</summary>
		public static ArrayList GetPaths(ArrayList docNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ArrayList>(MethodBase.GetCurrentMethod(),docNums);
			}
			if(docNums.Count==0){
				return new ArrayList();
			}
			string command="SELECT document.DocNum,document.FileName,patient.ImageFolder "
				+"FROM document "
				+"LEFT JOIN patient ON patient.PatNum=document.PatNum "
				+"WHERE document.DocNum = '"+docNums[0].ToString()+"'";
			for(int i=1;i<docNums.Count;i++){
				command+=" OR document.DocNum = '"+docNums[i].ToString()+"'";
			}
			//remember, they will not be in the correct order.
			DataTable table=Db.GetTable(command);
			Hashtable hList=new Hashtable();//key=docNum, value=path
			//one row for each document, but in the wrong order
			for(int i=0;i<table.Rows.Count;i++){
				//We do not need to check if A to Z folders are being used here, because
				//thumbnails are not visible from the chart module when A to Z are disabled,
				//making it impossible to launch the form image viewer (the only place this
				//function is called from.
				hList.Add(PIn.Long(table.Rows[i][0].ToString()),
					ODFileUtils.CombinePaths(new string[] {	ImageStore.GetPreferredImagePath(),
																									PIn.String(table.Rows[i][2].ToString()).Substring(0,1).ToUpper(),
																									PIn.String(table.Rows[i][2].ToString()),
																									PIn.String(table.Rows[i][1].ToString()),}));
			}
			ArrayList retVal=new ArrayList();
			for(int i=0;i<docNums.Count;i++){
				retVal.Add((string)hList[(long)docNums[i]]);
			}
			return retVal;
		}
		
		///<summary>Will return null if no picture for this patient.</summary>
		public static Document GetPatPictFromDb(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Document>(MethodBase.GetCurrentMethod(),patNum);
			} 
			//first establish which category pat pics are in
			long defNumPicts=0;
			Def[] defs=DefC.GetList(DefCat.ImageCats);
			for(int i=0;i<defs.Length;i++){
				if(Regex.IsMatch(defs[i].ItemValue,@"P")){
					defNumPicts=defs[i].DefNum;
					break;
				}
			}
			if(defNumPicts==0){//no category set for picts
				return null;
			}
			//then find, limit 1 to get the most recent
			string command="SELECT * FROM document "
				+"WHERE document.PatNum="+POut.Long(patNum)
				+" AND document.DocCategory="+POut.Long(defNumPicts)
				+" ORDER BY DateCreated DESC";
			command=DbHelper.LimitOrderBy(command,1);
			DataTable table=Db.GetTable(command);
			Document[] pictureDocs=Fill(table);
			if(pictureDocs==null || pictureDocs.Length<1){//no pictures
				return null;
			}
			return pictureDocs[0];
		}

		/// <summary>Makes one call to the database to retrieve the document of the patient for the given patNum, then uses that document and the patFolder to load and process the patient picture so it appears the same way it did in the image module.  It first creates a 100x100 thumbnail if needed, then it uses the thumbnail so no scaling needed. Returns false if there is no patient picture, true otherwise. Sets the value of patientPict equal to a new instance of the patient's processed picture, but will be set to null on error. Assumes WithPat will always be same as patnum.</summary>
		public static bool GetPatPict(long patNum,string patFolder,out Bitmap patientPict) {
			//No need to check RemotingRole; no call to db.
			Document pictureDoc=GetPatPictFromDb(patNum);
			if(pictureDoc==null) {
				patientPict=null;
				return false;
			}
			patientPict=GetThumbnail(pictureDoc,patFolder,100);
			return (patientPict!=null);
		}

		///<summary>Gets the corresponding thumbnail image for the given document object. The document is expected to be an image, and a 'not available' image is returned if the document is not an image. The thumbnail for every document is in a folder named 'thumbnails' within the same directly level.</summary>
		public static Bitmap GetThumbnail(Document doc,string patFolder,int size){
			//No need to check RemotingRole; no call to db.
			string shortFileName=doc.FileName;
			//If no file name associated with the document, then there cannot be a thumbnail,
			//because thumbnails have the same name as the original image document.
			if(shortFileName.Length<1) {
				return NoAvailablePhoto(size);
			}
			string fullName=ODFileUtils.CombinePaths(patFolder,shortFileName);
			//If the document no longer exists, then there is no corresponding thumbnail image.
			if(!File.Exists(fullName)) {
				return NoAvailablePhoto(size);
			}
			//If the specified document is not an image return 'not available'.
			if(!ImageHelper.HasImageExtension(fullName)) {
				return NoAvailablePhoto(size);
			}
			//Create Thumbnails folder if it does not already exist for this patient folder.
			string thumbPath=ODFileUtils.CombinePaths(patFolder,"Thumbnails");
			if(!Directory.Exists(thumbPath)) {
				try {
					Directory.CreateDirectory(thumbPath);
				} 
				catch {
					throw new ApplicationException(Lans.g("Documents","Error: Could not create 'Thumbnails' folder for patient: ")+thumbPath);
				}
			}
			string thumbFileExt=Path.GetExtension(shortFileName);
			string thumbCoreFileName=shortFileName.Substring(0,shortFileName.Length-thumbFileExt.Length);
			string thumbFileName=ODFileUtils.CombinePaths(new string[] { patFolder,"Thumbnails",
				thumbCoreFileName+"_"+size+thumbFileExt} );
			//Use the existing thumbnail if it already exists and it was created after the last document modification.
			if(File.Exists(thumbFileName)) {
				DateTime thumbModifiedTime=File.GetLastWriteTime(thumbFileName);
				if(thumbModifiedTime>doc.DateTStamp){
					return (Bitmap)Bitmap.FromFile(thumbFileName);
				}
			}
			//Add thumbnail
			Bitmap thumbBitmap;
			//Gets the cropped/flipped/rotated image with any color filtering applied.
			Bitmap sourceImage=new Bitmap(fullName);
			Bitmap fullImage=ImageHelper.ApplyDocumentSettingsToImage(doc,sourceImage,ApplyImageSettings.ALL);
			sourceImage.Dispose();
			thumbBitmap=ImageHelper.GetThumbnail(fullImage,size);
			fullImage.Dispose();
			try {
				thumbBitmap.Save(thumbFileName);
			} catch {
				//Oh well, we can regenerate it next time if we have to!
			}
			return thumbBitmap;
		}

		public static Bitmap NoAvailablePhoto(int size){
			//No need to check RemotingRole; no call to db.
			Bitmap bitmap=new Bitmap(size,size);
			Graphics g=Graphics.FromImage(bitmap);
			g.InterpolationMode=InterpolationMode.High;
			g.FillRectangle(Brushes.Gray,0,0,bitmap.Width,bitmap.Height);
			StringFormat notAvailFormat=new StringFormat();
			notAvailFormat.Alignment=StringAlignment.Center;
			notAvailFormat.LineAlignment=StringAlignment.Center;
			Font font=new Font("Courier New",8.25F,FontStyle.Regular,GraphicsUnit.Point,((byte)(0)));
			g.DrawString("Thumbnail not available",font,Brushes.Black,
				new RectangleF(0,0,size,size),notAvailFormat);
			g.Dispose();
			return bitmap;
		}

		///<summary>Returns the documents which correspond to the given mountitems.</summary>
		public static Document[] GetDocumentsForMountItems(List <MountItem> mountItems) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Document[]>(MethodBase.GetCurrentMethod(),mountItems);
			}
			if(mountItems==null || mountItems.Count<1){
				return new Document[0];
			}
			Document[] documents=new Document[mountItems.Count];
			for(int i=0;i<mountItems.Count;i++){
				string command="SELECT * FROM document WHERE MountItemNum='"+POut.Long(mountItems[i].MountItemNum)+"'";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count<1){
					documents[i]=null;
				}else{
					documents[i]=Fill(table)[0];
				}
			}
			return documents;
		}

		///<summary>Any filenames mentioned in the fileList which are not attached to the given patient are properly attached to that patient. Returns the total number of documents that were newly attached to the patient.</summary>
		public static int InsertMissing(Patient patient,List<string> fileList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patient,fileList);
			}
			int countAdded=0;
			string command="SELECT FileName FROM document WHERE PatNum='"+patient.PatNum+"' ORDER BY FileName";
			DataTable table=Db.GetTable(command);
			for(int j=0;j<fileList.Count;j++){
				string fileName=Path.GetFileName(fileList[j]);
				if(!IsAcceptableFileName(fileName)){
					continue;
				}
				bool inList=false;
				for(int i=0;i<table.Rows.Count && !inList;i++){
					inList=(table.Rows[i]["FileName"].ToString()==fileName);
				}
				if(!inList){
					Document doc=new Document();
					doc.DateCreated=File.GetLastWriteTime(fileList[j]);
					doc.Description=fileName;
					doc.DocCategory=DefC.GetList(DefCat.ImageCats)[0].DefNum;//First category.
					doc.FileName=fileName;
					doc.PatNum=patient.PatNum;
					Insert(doc,patient);
					countAdded++;
				}
			}
			return countAdded;
		}

		///<Summary>Parameters: 1:PatNum</Summary>
		public static DataSet RefreshForPatient(string[] parameters) {
			//No need to check RemotingRole; no call to db.
			DataSet retVal=new DataSet();
			retVal.Tables.Add(GetTreeListTableForPatient(parameters[0]));
			return retVal;
		}

		public static DataTable GetTreeListTableForPatient(string patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("DocumentList");
			DataRow row;
			DataTable raw;
			string command;
			//Rows are first added to the resultSet list so they can be sorted at the end as a larger group, then
			//they are placed in the datatable to be returned.
			List<Object> resultSet=new List<Object>();
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("DocNum");
			table.Columns.Add("MountNum");
			table.Columns.Add("DocCategory");
			table.Columns.Add("DateCreated");
			table.Columns.Add("docFolder");//The folder order to which the Document category corresponds.
			table.Columns.Add("description");
			table.Columns.Add("ImgType");
			//Move all documents which are invisible to the first document category.
			command="SELECT DocNum FROM document WHERE PatNum='"+patNum+"' AND "
				+"DocCategory<0";
			raw=dcon.GetTable(command);
			if(raw.Rows.Count>0){//Are there any invisible documents?
				command="UPDATE document SET DocCategory='"+DefC.GetList(DefCat.ImageCats)[0].DefNum
					+"' WHERE PatNum='"+patNum+"' AND (";
				for(int i=0;i<raw.Rows.Count;i++){
					command+="DocNum='"+PIn.Long(raw.Rows[i]["DocNum"].ToString())+"' ";
					if(i<raw.Rows.Count-1){
						command+="OR ";
					}
				}
				command+=")";
				dcon.NonQ(command);
			}
			//Load all documents into the result table.
			command="SELECT DocNum,DocCategory,DateCreated,Description,ImgType,MountItemNum FROM document WHERE PatNum='"+patNum+"'";
			raw=dcon.GetTable(command);
			for(int i=0;i<raw.Rows.Count;i++){
				//Make sure hidden documents are never added (there is a small possibility that one is added after all are made visible).
				if(DefC.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()))<0){ 
					continue;
				}
				//Do not add individual documents which are part of a mount object.
				if(PIn.Long(raw.Rows[i]["MountItemNum"].ToString())!=0) {
					continue;
				}
				row=table.NewRow();
				row["DocNum"]=PIn.Long(raw.Rows[i]["DocNum"].ToString());
				row["MountNum"]=0;
				row["DocCategory"]=PIn.Long(raw.Rows[i]["DocCategory"].ToString());
				row["DateCreated"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString());
				row["docFolder"]=DefC.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()));
				row["description"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "
					+PIn.String(raw.Rows[i]["Description"].ToString());
				row["ImgType"]=PIn.Long(raw.Rows[i]["ImgType"].ToString());
				resultSet.Add(row);
			}
			//Move all mounts which are invisible to the first document category.
			command="SELECT MountNum FROM mount WHERE PatNum='"+patNum+"' AND "
				+"DocCategory<0";
			raw=dcon.GetTable(command);
			if(raw.Rows.Count>0) {//Are there any invisible mounts?
				command="UPDATE mount SET DocCategory='"+DefC.GetList(DefCat.ImageCats)[0].DefNum
					+"' WHERE PatNum='"+patNum+"' AND (";
				for(int i=0;i<raw.Rows.Count;i++) {
					command+="MountNum='"+PIn.Long(raw.Rows[i]["MountNum"].ToString())+"' ";
					if(i<raw.Rows.Count-1) {
						command+="OR ";
					}
				}
				command+=")";
				dcon.NonQ(command);
			}
			//Load all mounts into the result table.
			command="SELECT MountNum,DocCategory,DateCreated,Description,ImgType FROM mount WHERE PatNum='"+patNum+"'";
			raw=dcon.GetTable(command);
			for(int i=0;i<raw.Rows.Count;i++){
				//Make sure hidden mounts are never added (there is a small possibility that one is added after all are made visible).
				if(DefC.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()))<0) {
					continue;
				}
				row=table.NewRow();
				row["DocNum"]=0;
				row["MountNum"]=PIn.Long(raw.Rows[i]["MountNum"].ToString());
				row["DocCategory"]=PIn.Long(raw.Rows[i]["DocCategory"].ToString());
				row["DateCreated"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString());
				row["docFolder"]=DefC.GetOrder(DefCat.ImageCats,PIn.Long(raw.Rows[i]["DocCategory"].ToString()));
				row["description"]=PIn.Date(raw.Rows[i]["DateCreated"].ToString()).ToString("d")+": "
					+PIn.String(raw.Rows[i]["Description"].ToString());
				row["ImgType"]=PIn.Long(raw.Rows[i]["ImgType"].ToString());
				resultSet.Add(row);
			}
			//We must sort the results after they are returned from the database, because the database software (i.e. MySQL)
			//cannot return sorted results from two or more result sets like we have here.
			resultSet.Sort(delegate(Object o1,Object o2) {
				DataRow r1=(DataRow)o1;
				DataRow r2=(DataRow)o2;
				int docFolder1=Convert.ToInt32(r1["docFolder"].ToString());
				int docFolder2=Convert.ToInt32(r2["docFolder"].ToString());
				if(docFolder1<docFolder2){
					return -1;
				}else if(docFolder1>docFolder2){
					return 1;
				}
				return PIn.Date(r1["DateCreated"].ToString()).CompareTo(PIn.Date(r2["DateCreated"].ToString()));
			});
			//Finally, move the results from the list into a data table.
			for(int i=0;i<resultSet.Count;i++){
				table.Rows.Add((DataRow)resultSet[i]);
			}
			return table;
		}

		///<summary>Returns false if the file is a specific short file name that is not accepted or contains one of the unsupported file exentions.</summary>
		public static bool IsAcceptableFileName(string file) {
			//No need to check RemotingRole; no call to db.
			string[] specificBadFileNames=new string[] {
				"thumbs.db"
			};
			for(int i=0;i<specificBadFileNames.Length;i++) {
				if(file.Length>=specificBadFileNames[i].Length && 
					file.Substring(file.Length-specificBadFileNames[i].Length,
					specificBadFileNames[i].Length).ToLower()==specificBadFileNames[i]) {
					return false;
				}
			}
			return true;
		}

		///<summary>When first opening the image module, this tests to see whether a given filename is in the database. Also used when naming a new document to ensure unique name.</summary>
		public static bool IsFileNameInList(string fileName,string[] usedNames) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<usedNames.Length;i++) {
				if(usedNames[i]==fileName)
					return true;
			}
			return false;
		}

		//public static string GetFull

		///<summary>Only documents listed in the corresponding rows of the statement table are uploaded</summary>
		public static List<long> GetChangedSinceDocumentNums(DateTime changedSince,List<long> statementNumList) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,statementNumList);
			}
			string strStatementNums="";
			DataTable table;
			if(statementNumList.Count>0) {
				for(int i=0;i<statementNumList.Count;i++) {
					if(i>0) {
						strStatementNums+="OR ";
					}
					strStatementNums+="StatementNum='"+statementNumList[i].ToString()+"' ";
				}
				string command="SELECT DocNum FROM document WHERE  DateTStamp > "+POut.DateT(changedSince)+" AND DocNum IN ( SELECT DocNum FROM statement WHERE "+strStatementNums+")";
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<long> documentnums = new List<long>(table.Rows.Count);
			for(int i=0;i<table.Rows.Count;i++) {
				documentnums.Add(PIn.Long(table.Rows[i]["DocNum"].ToString()));
			}
			return documentnums;
		}

		///<summary>Used along with GetChangedSinceDocumentNums</summary>
		public static List<Document> GetMultDocuments(List<long> documentNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Document>>(MethodBase.GetCurrentMethod(),documentNums);
			}
			string strDocumentNums="";
			DataTable table;
			if(documentNums.Count>0) {
				for(int i=0;i<documentNums.Count;i++) {
					if(i>0) {
						strDocumentNums+="OR ";
					}
					strDocumentNums+="DocNum='"+documentNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM document WHERE "+strDocumentNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Document[] multDocuments=Crud.DocumentCrud.TableToList(table).ToArray();
			List<Document> documentList=new List<Document>(multDocuments);
			foreach(Document d in documentList){
				if(string.IsNullOrEmpty(d.RawBase64)){
					Patient pat=Patients.GetPat(d.PatNum);
					string patFolder=ImageStore.GetPatientFolder(pat);
					string filePathAndName=ImageStore.GetFilePath(Documents.GetByNum(d.DocNum),patFolder);
					if(File.Exists(filePathAndName)) {
						FileStream fs= new FileStream(filePathAndName,FileMode.Open,FileAccess.Read);
						byte[] rawData = new byte[fs.Length];
						fs.Read(rawData,0,(int)fs.Length);
						fs.Close();
						d.RawBase64=Convert.ToBase64String(rawData);
					}
				}
			}
			return documentList;
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all documents of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE document SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

	}	
  
}