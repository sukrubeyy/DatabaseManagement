# Database Management Tool For Unity (UDBM v2)

<ul>

<li>
  <h2>Overview</h2>
  <p>
    <strong>Unity Database Management Tool (UDBM vNext)</strong> is a database design and management tool developed for Unity.
    It allows developers to visually design database structures inside Unity Editor and generate database schemas using
    custom entity definitions and migrations.
  </p>
</li>

<li>
  <h2>Genel Bakış (TR)</h2>
  <p>
    <strong>Unity Database Management Tool (UDBM vNext)</strong>, Unity Editor içerisinde veritabanı yapılarını
    tasarlamanızı, entity’ler oluşturmanızı ve migration işlemleriyle tablolarınızı üretmenizi sağlayan bir araçtır.
    Sistem, NestJS tabanlı asenkron bir API ile desteklenmektedir.
  </p>
</li>

<li>
  <h2>Core Architecture</h2>
  <ul>
    <li>Asynchronous backend architecture powered by <strong>NestJS API</strong></li>
    <li>Modern and flexible UI built with <strong>Unity UI Toolkit</strong></li>
    <li>Visual database diagrams using <strong>GraphView</strong></li>
    <li>Custom Entity & Attribute based database design</li>
    <li>Editor-based migration system</li>
  </ul>
</li>

<li>
  <h2>Temel Mimari (TR)</h2>
  <ul>
    <li><strong>NestJS</strong> tabanlı asenkron API mimarisi</li>
    <li><strong>UI Toolkit</strong> ile geliştirilen esnek ve modern arayüz</li>
    <li><strong>GraphView</strong> üzerinden görsel veritabanı diyagramları</li>
    <li>Custom Entity & Attribute tabanlı veritabanı tasarımı</li>
    <li>Unity Editor üzerinden migration sistemi</li>
  </ul>
</li>

<li>
  <h2>Entity & Migration System</h2>
  <p>
    Developers can create entity classes under the <strong>UDBM/Entities</strong> directory.
    Using custom attributes, database tables, columns, and relationships can be defined directly in code.
    These definitions can then be migrated through the Unity Editor to generate database schemas.
  </p>
</li>

<li>
  <h2>Entity & Migration Sistemi (TR)</h2>
  <p>
    Geliştiriciler <strong>UDBM/Entities</strong> dizini altında entity yapılarını oluşturabilir.
    Tasarlanan custom attribute’ler sayesinde tablolar, kolonlar ve ilişkiler kod üzerinden tanımlanır.
    Unity Editor içinden migration işlemi çalıştırılarak veritabanı yapısı otomatik olarak oluşturulur.
  </p>
</li>

<li>
  <h2>Supported Databases</h2>
  <ul>
    <li>SQLite</li>
    <li>MongoDB</li>
    <li>MySQL</li>
    <li>Microsoft SQL Server (MSSQL)</li>
    <li>PostgreSQL</li>
  </ul>
</li>

<li>
  <h2>Desteklenen Veritabanları (TR)</h2>
  <ul>
    <li>SQLite</li>
    <li>MongoDB</li>
    <li>MySQL</li>
    <li>Microsoft SQL Server (MSSQL)</li>
    <li>PostgreSQL</li>
  </ul>
</li>

<li>
  <h2>Version Notes</h2>
  <p>
    Previous versions supported only SQLite and MongoDB.
    In this version, MySQL, MSSQL, and PostgreSQL support has been added.
  </p>
</li>

<li>
  <h2>Sürüm Notları (TR)</h2>
  <p>
    Önceki sürümde yalnızca SQLite ve MongoDB desteği bulunmaktaydı.
    Bu sürüm ile birlikte MySQL, MSSQL ve PostgreSQL destekleri eklenmiştir.
  </p>
</li>

<li>
  <h2>Development Status</h2>
  <p>
    The NestJS API is currently under development and has not been published yet.
    Therefore, the tool is not fully functional at the moment.
    Both the API and Unity Editor tool are actively being developed.
  </p>
</li>

<li>
  <h2>Geliştirme Durumu (TR)</h2>
  <p>
    NestJS API henüz yayınlanmamıştır ve geliştirme aşamasındadır.
    Bu nedenle tool şu anda tam olarak çalışır durumda değildir.
    Hem API hem de Unity Editor tool’u aktif olarak geliştirilmektedir.
  </p>
</li>

</ul>
