create schema ad_util_GeneralPurposeTree authorization dbo;
go

create table [ad_util_GeneralPurposeTree].[Root](
    Id bigint identity not null,
    Name nvarchar(250) not null default(''),
    Initiator nvarchar(250) not null);
go

alter table [ad_util_GeneralPurposeTree].[Root] add constraint [PK_Root] primary key (Id);
go

create unique index [UK_Root_Name] on [ad_util_GeneralPurposeTree].[Root] (Name);
go

-- AUTOPROC All [ad_util_GeneralPurposeTree].[Root]
go

create table [ad_util_GeneralPurposeTree].[Node](
    RootId bigint not null,
    Id bigint identity not null,
    ParentId bigint,
    Name nvarchar(250) not null default(''));
go

alter table [ad_util_GeneralPurposeTree].[Node] add constraint [PK_Node] primary key (Id);
go

alter table [ad_util_GeneralPurposeTree].[Node] add constraint [FK_Node_Root] foreign key (RootId) references [ad_util_GeneralPurposeTree].[Root](Id);
go

alter table [ad_util_GeneralPurposeTree].[Node] add constraint [FK_Node_Node] foreign key (ParentId) references [ad_util_GeneralPurposeTree].[Node](Id);
go

create unique index [UK_Node_Name] on [ad_util_GeneralPurposeTree].[Node] (Name);
go

create index [ad_util_GeneralPurposeTree__IX_Node_ParentId] on [ad_util_GeneralPurposeTree].[Node](ParentId);
go

CREATE PROCEDURE [ad_util_GeneralPurposeTree].[GetTreeUnderRoot] 
    @rootId bigint, @maxLevel int = 15
AS
BEGIN
    declare @levelIdWidth int;
    set @levelIdWidth=9;
    with InnerChildrenQuery as (
        select *, 0 level, cast(right(REPLICATE(N' ', @levelIdWidth)+ltrim(rtrim(str(Id))), @levelIdWidth) as nvarchar(max)) item_id
        from ad_util_GeneralPurposeTree.Node
        where RootId = @rootId and ParentId is null

        union all

        select b.*, c.level + 1 level, c.item_id+N'.'+right(REPLICATE(N' ', @levelIdWidth)+ltrim(rtrim(str(b.Id))), @levelIdWidth) item_id
        from ad_util_GeneralPurposeTree.Node b join InnerChildrenQuery c on b.ParentId = c.Id
        where @maxLevel > 1 and level < @maxLevel
        )
    select * from InnerChildrenQuery 
    order by item_id, level;
END
GO

CREATE PROCEDURE [ad_util_GeneralPurposeTree].[GetTreeUnderNode] 
    @id bigint, @maxLevel int = 15
AS
BEGIN
    declare @levelIdWidth int;
    set @levelIdWidth=9;
    with InnerChildrenQuery as (
        select *, 0 level, cast(right(REPLICATE(N' ', @levelIdWidth)+ltrim(rtrim(str(Id))), @levelIdWidth) as nvarchar(max)) item_id
        from ad_util_GeneralPurposeTree.Node
        where ParentId = @id

        union all

        select b.*, c.level + 1 level, c.item_id+N'.'+right(REPLICATE(N' ', @levelIdWidth)+ltrim(rtrim(str(b.Id))), @levelIdWidth) item_id
        from ad_util_GeneralPurposeTree.Node b join InnerChildrenQuery c on b.ParentId = c.Id
        where @maxLevel > 1 and level < @maxLevel
        )
    select * from InnerChildrenQuery 
    order by item_id, level;
END
GO

-- AUTOPROC All [ad_util_GeneralPurposeTree].[Node]
go

