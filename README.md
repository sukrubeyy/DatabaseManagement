# Database Management Tool For Unity  

<ul>
  <li>
    <h2>MongoDB</h2>
     <h4>Package Description</h4>
    <p>This package is used to control mongodb database, collections and items. You are comfortable do CRUD operation this tool. This tool when connection your mongo atlas databases on this tool create a json file based your databases. If json file exist in your asset folder then open Mongolist View Window and you can crud operation on this window. When you want to send changes made locally to the cloud, you have to use " Send Json To Cloud " button on MongoList window or update MongoList windows json file then you have to use " Reset " button, if you want to update json file based your mongo atlas you have to use " Update Json File " button.
      I have explained in detail how to use this tool below. If you are catch any bug you can create an issue, send email or message to me.  
  </p>
  <ul>
      <li>Connection to MongoDB Atlas</li>
    <ol>
      <li>Click Database Management Menu</li>
      <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/ToolMenuItem.PNG"></img>
      <li>Click Database Connection</li>
       <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/connectionMongo.PNG"></img>
      <li>Choose MongoDb</li>
      <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/toolManagementWindow.PNG"></img>
      <li>Write your MongoDB Atlas connection url and press connect button. If the connection url is not wrong, the databases connected to the mongodb atlas are created as json file.</li>
      <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/MongoListWindow.png"></img>  
    </ol>  
  <li>Managemenet Database, Collection and items</li>
  <ol>
    <li>Database Management</li>
    <p>
        As you can see in the image below, your databases are listed in the left window. When you press any database, collections based on that database are          listed in the middle window. If you click on any collection, all items based on that collection are listed in the right window.
    </p>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/MongoListWindow.png"></img>  
    <p>
      If you want to create a new database then click Create a new database button to left window and enter the database name.
      If you want to delete database click X button.
      Click "Restart" button.
    </p>
     <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createNewDatabase.png"></img>
    <li>Collection Management</li>   
     <p>
      If you want to create a new collection then click Create a new collection button to middle window and enter the collection name.
      If you want to delete collection click X button.
      Click "Restart" button.
     </p>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createNewCollection.png"></img>
    <li>Items Management</li> 
  <ul>
       <li>Create</li>
          <p>
        If you want to create a new item then click Create a new item button to right window.
        If collection exist any item you have to create item based these items. 
        <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createNewItemIfExist.png"></img>
          </p>
   <p>
    But does not exist any item then you entry input field count and entry 
    property name - property value.
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createNewItemIfNotExist.png"></img>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createItemNotExist2.png"></img>
    Click "Restart" button.
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/main/images/createItemNotExist2.png"></img>
   </p>
       <li>Update</li>
         <p>
        If you want to update item then click "✓" button below of item.
         </p>
       <li>Delete</li>
          <p>
        If you want to delete item then click "X" button below of item.
         </p>
      <li>Reset</li>
          <p>
        If you want to reset item then click "<->" button below of item.
         </p>
  </ul 
  </ol>
  </ul>
  </li>


<li>
    <h2>SQLite</h2>
     <h4>Package Description</h4>
    <p>This package is used to control SQLite database, table and items. You are comfortable do CRUD operation this tool. 
      I have explained in detail how to use this tool below. 
  </p>

<ul>
      <li>Click Database Management Menu</li>
    <ol>
      <li>Click SQLite View Menu</li>
       <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/SQLiteViewMENU.PNG"/>
       
  </ol>  
  <li>Managemenet Database, Table And Items</li>
  <ol>
    <li>Database Management</li>
    <p>
        As you can see in the image below, your databases are listed in the left window. When you press any database, tables based on that database are          listed in the middle window. If you click on any table, all items based on that tables are listed in the right window.
    </p>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/SQLite%20View%20Window.PNG"/>  
    <p>
      If you want to create a new database then click Create a new database button to left window and enter the database name.
      You have to create datababse in this directory Assets/Scripts/SQLite/Databases/ if you are using DB Browser or like this programs.
      If you want to delete database click X button.
    </p>
     <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createDatabase1.PNG"/>
    <br>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createDatabase1.PNG"/>
    <li>Table Management</li>   
     <p>
      If you want to create a new table then click Create a new table button to middle window and enter the table name.
      If you want to delete table click X button.
     </p>
   <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createTable1.PNG"/>
    <br>
   <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createTable2.PNG"/>
    <li>Items Management</li> 
  <ul>
       <li>Create</li>
          <p>
        If you want to create a new item then click Create a new item button to right window.
        If table exist any item you have to create item based these items. 
        <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createItemIfExist1.PNG"/>
        <br>
        <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createItemIfExist2.PNG"/>
        <br>
        <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createItemIfExist3.PNG"/>
          </p>
   <p>
    But does not exist any item then you entry input field count and entry 
    property type - property name and when click save then you can entry item values.
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createItemNotExist1.PNG"/>
     <br>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createItemNotExist2.PNG"></img>
    <br>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/createdItem.PNG"></img>
    <br>
    <p> IF YOU HAVE NOT ANY TABLE AND YOU TRY CREATE A NEW ITEM YOU GET THIS MESSAGE</p>
    <img src="https://github.com/sukrubeyy/DatabaseManagement/blob/SQLite/SQLiteImages/CreateItemIfNotExistAnyTable.PNG">
   </p>
       <li>Update</li>
         <p>
        If you want to update item then click "✓" button below of item.
         </p>
       <li>Delete</li>
          <p>
        If you want to delete item then click "X" button below of item.
         </p>
      <li>Reset</li>
          <p>
        If you want to refresh items then click "<->" button below of item.
         </p>
  </ul 
  </ol>
  </ul>
  </li>





</ul>
