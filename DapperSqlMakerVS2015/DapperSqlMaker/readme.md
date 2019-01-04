**DapperSqlMaker ��ʽ��ѯ��չ** 

[Gihub��ַ](https://github.com/mumumutou/DapperSqlMaker)
###### Nuget��װ:  
	Install-Package DapperSqlMaker -Version 0.0.2

###### ����(������Դ��):
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib
###### Demo:
	��ѯ       TestsDapperSqlMaker\DapperSqlMaker.Test\  SelectDapperSqlMakerTest.cs
	���       TestsDapperSqlMaker\DapperSqlMaker.Test\  InsertDapperSqlMakerTest.cs
	����       TestsDapperSqlMaker\DapperSqlMaker.Test\  UpdateDapperSqlMakerTest.cs
	ɾ��       TestsDapperSqlMaker\DapperSqlMaker.Test\  DeleteDapperSqlMakerTest.cs
	��������   TestsDapperSqlMaker\DbDapperSqlMaker\     LockDapperUtilsqlite.cs
	
##### �����ӣ�

##### 1.��ѯ-�����ѯ,��ҳ

```csharp 
public void ���������ҳ����()
{
    var arruser = new int[2] { 1,2 };  // 
    string uall = "b.*", pn1 = "%����%", pn2 = "%m%";
    LockPers lpmodel = new LockPers() { IsDel = false};
    Users umodel = new Users() { UserName = "jiaojiao" };
    SynNote snmodel = new SynNote() { Name = "ľͷ" };
    Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
    where = where.And((l, u, s) => ( l.Name.Contains(pn1) || l.Name.Contains(pn2) ));
    where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
    where = where.And((l, u, s) => u.UserName == umodel.UserName);
    where = where.And((l, u, s) => s.Name == snmodel.Name );
    where = where.And((l, u, s) => SM.In(u.Id, arruser));

    DapperSqlMaker<LockPers, Users, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote>
        .Selec()
        .Column((lp, u, s) => //null)  //��ѯ�����ֶ�
            new { lp.Name, lpid = lp.Id, x = "LENGTH(a.Prompt) as len", b = SM.Sql(uall), scontent = s.Content, sname = s.Name })
        .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
        .Where(where)
        .Order((lp, w, sn) => new { lp.EditCount, x = SM.OrderDesc(lp.Name), sn.Content });

    var result = query.ExcuteSelect();
    WriteJson(result); //  ��ѯ���

    Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
    WriteSqlParams(resultsqlparams); // ��ӡsql�Ͳ���

    int page = 2, rows = 3, records;
    var result2 = query.LoadPagelt(page, rows, out records);
    WriteJson(result2); //  ��ѯ���
}
```
*���ɵ�sql* :
```sql
select  a.Name as Name, a.Id as lpid
	, LENGTH(a.Prompt) as len, b.*
	, c.Content as scontent, c.Name as sname  
from LockPers a  
	left join  Users b on  b.Id = a.UserId   
	inner join  SynNote c on  b.Id = c.UserId  
where  (  a.Name like @Name0  or  a.Name like @Name1  )  
	and  a.IsDel = @IsDel2  and  b.UserName = @UserName3  
	and  c.Name = @Name4  and  b.Id in @Id 
order by  a.EditCount, a.Name desc , c.Content 
```

##### 2.����-���²����ֶ�

```csharp
public void ���²����ֶβ���lt()
{
    var issucs = LockDapperUtilsqlite<LockPers>.Cud.Update(
        s =>
        {
            s.Name = "����bool�޸�1";
            s.Content = "update�����ڸ�ֵ�޸��ֶ�";
            s.IsDel = true;
        },
        w => w.Name == "����bool�޸�1" && w.IsDel == true
        );
    Console.WriteLine(issucs);
}
```


 

-----

 
 ע�⣺
> 1. svn�ύ��githubʱ ��Ҫ�ٽ�������ڸ����ļ� ֱ�ӵ������ļ���ӽ��� 
> 2. ���Դ�ӡdapper��ѯsql  
   ����: Dapper.SqlMapper.QueryImpl  
    ȡ��ע��: // Console.WriteLine(cmd.CommandText);  
> 3. where����
     �ɱ���� �Ƚ�ʱ ��ת��ֵ����
> 4. ʵ�������׺��Ҫ������
> 5. ������� a,b,c... ˳������
> 6. ���������ϴ���չ       
    ֻ��copy��ʵ�ֵ� �޸�3���ļ�            
    DapperSqlMaker              
    Template_DapperSqlMaker ��������         
    PredicateBuilder        ����ƴ����
> 7. [ʵ������T4ģ��ʹ�÷�������](https://www.cnblogs.com/cl-blogs/p/7205954.html)
>
