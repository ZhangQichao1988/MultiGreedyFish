# coding:utf-8

from __future__ import division
from __future__ import print_function

import codecs
import collections
import os
import shutil
import sys
import time

import xlrd

try:  # python3
    import configparser
except:  # python2
    import ConfigParser as configparser

reload(sys)
sys.setdefaultencoding('utf8')

PATH_JSON_DATA = "Assets/Resources/JsonData/"
PATH_XLSX = "xlsx/"
PATH_SQL_OUTPUT = "dict_dql/"


def parse_exl_client(exl_path, table_name):
    f = codecs.open(os.path.join(json_output, table_name) + ".json", "w", "utf-8")
    # f.write(u"local tableName = {\n\t\"items\":[\n")

    print(u"开始导出client" + table_name)
    print(exl_path)

    workbook = xlrd.open_workbook(exl_path)
    # 获取sheet
    sheet_name = workbook.sheet_names()[0]
    sheet = workbook.sheet_by_name(sheet_name)

    list_head = sheet.row_values(0)

    f.write(u"{\"Items\":[\n")
    # 获取一行的内容
    for i in range(1, sheet.nrows):
        temp = []

        for index in range(0, len(list_head)):
            v = sheet.cell(i, index).value
            temp.append(str(v))
            # if isinstance(v, basestring):
            #     temp.append(str(v).strip().decode("utf-8"))
            # else:
            #     temp.append(v)

        f.write(u"\t{")

        for n in range(0, len(temp)):
            if list_head[n].strip() == "" or list_head[n] == "ID" and temp[n].strip() == "":
                continue

            f.write(u"\"" + list_head[n] + "\":\"" + temp[n] + "\"")
            if n < len(temp) - 1:
                f.write(u",")
            else:
                if i < sheet.nrows - 1:
                    f.write(u"},\n")
                else:
                    f.write(u"}\n")

    f.write(u"]}")
    f.close()


create_database = "DROP DATABASE IF EXISTS fish_dict;" + "\n" + "CREATE DATABASE IF NOT EXISTS fish_dict DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;" + "\n" + "USE fish_dict;\n"
drop_sql = "DROP TABLE IF EXISTS `{}`;";
create_sql = "CREATE TABLE `{}`({}PRIMARY KEY (`ID`))ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"
insert_sql = "INSERT INTO `{}`({}) VALUES {};"


def sort_cloum_item(arr):
    dic = collections.OrderedDict()
    for item in range(0, len(arr), 2):
        dic[arr[item]] = arr[item + 1]

    return dic


base_data_type = {
    "Config": sort_cloum_item(["ID", "INT(11)", "key", "TEXT", "value", "DOUBLE(16,2)"]),
    "EffectData": sort_cloum_item(["ID", "INT(11)", "prefabPath", "TEXT", "duration", "DOUBLE(16,2)"]),
    "FishBuffData": sort_cloum_item(["ID", "INT(11)", "buffType", "TEXT", "aryParam", "TEXT"]),
    "FishData": sort_cloum_item(
        ["ID", "INT(11)", "name", "TEXT", "prefabPath", "TEXT", "atk", "INT(11)", "life", "INT(11)",
         "moveSpeed", "DOUBLE(16,2)", "skillId", "INT(11)"]),
    "FishSkillData": sort_cloum_item(["ID", "INT(11)", "skillType", "TEXT", "effectId", "INT(11)", "aryParam", "TEXT"]),
    "LanguageData": sort_cloum_item(["ID", "INT(11)", "cn", "TEXT", "tw", "TEXT", "en", "TEXT", "jp", "TEXT"]),
    "RobotAiData": sort_cloum_item(["ID", "INT(11)", "aiType", "TEXT", "aryParam", "TEXT"]),
    "RobotData": sort_cloum_item(["ID", "INT(11)", "fishId", "INT(11)", "aiId", "INT(11)", "groupId", "INT(11)"]),
    "EnemyData": sort_cloum_item(["ID", "INT(11)", "fishId", "INT(11)", "fishCountMin", "INT(11)",  "fishCountMax", "INT(11)",  "groupId", "INT(11)"]),
    "RobotGroupData": sort_cloum_item(["ID", "INT(11)", "groupId", "INT(11)", "rankMin", "INT(11)", "rankMax", "INT(11)"]),
}


def parse_exl_server(exl_path, table_name, f):
    print(u"开始导出server" + table_name)
    print(exl_path)

    workbook = xlrd.open_workbook(exl_path)
    # 获取sheet
    sheet_name = workbook.sheet_names()[0]
    sheet = workbook.sheet_by_name(sheet_name)

    list_head = sheet.row_values(0)

    f.write(drop_sql.format(table_name) + u"\n")
    f.write(create_sql.format(table_name, get_coloum(table_name)) + u"\n")

    insert_val = ""
    # 获取一行的内容
    for i in range(1, sheet.nrows):
        temp = []

        for index in range(0, len(list_head)):
            v = sheet.cell(i, index).value
            temp.append(str(v))
            # if isinstance(v, basestring):
            #     temp.append(str(v).strip().decode("utf-8"))
            # else:
            #     temp.append(v)

        row_val = "("
        for n in range(0, len(temp)):
            if list_head[n].strip() == "" or list_head[n] == "ID" and temp[n].strip() == "":
                continue

            if table_name not in base_data_type or list_head[n] not in base_data_type[table_name]:
                continue

            row_val += base_data_type[table_name][list_head[n]] == "TEXT" and "'" + temp[n] + "'" or temp[n]
            if n < len(temp) - 1:
                row_val += u","

        row_val += (i == sheet.nrows - 1) and ")" or "),"
        insert_val += row_val

    print(insert_sql.format(table_name, get_coloum_insert(table_name), insert_val))
    f.write(insert_sql.format(table_name, get_coloum_insert(table_name), insert_val) + "\n")


def get_coloum(tb_name):
    tb_info = base_data_type[tb_name]
    result = ""
    for key in tb_info:
        result += "`" + key + "` " + tb_info[key] + ","

    return result


def get_coloum_insert(tb_name):
    tb_info = base_data_type[tb_name]
    result = ""
    for key in tb_info:
        result += "`" + key + "`,"

    return result[:-1]


def isfloat(a):
    try:
        return float(a)
    except Exception as e:
        return False


def isint(a):
    try:
        return int(a)
    except Exception as e:
        return False


if __name__ == '__main__':
    start_time = time.time()

    _repository_root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
    json_output = os.path.join(_repository_root, PATH_JSON_DATA)
    xlsx_input = os.path.join(_repository_root, PATH_XLSX)
    sql_output = os.path.join(_repository_root, PATH_SQL_OUTPUT)
    try:
        if os.path.exists(json_output):
            shutil.rmtree(json_output)

        if os.path.exists(sql_output):
            shutil.rmtree(sql_output)

        os.makedirs(json_output)
        os.makedirs(sql_output)
    except Exception as e:
        pass

    sql_file = codecs.open(os.path.join(sql_output, "fish_dict.sql"), "w", "utf-8")
    sql_file.write(create_database)
    for root, dirs, files in os.walk(xlsx_input):
        for file in files:
            if ".xlsx" in file:
                file_name = file.split(".")[0]
                table_name = len(file_name.split("_")) > 1 and file_name.split("_")[1] or file_name
                is_server = len(file_name.split("_")) > 1 and "s" in file_name.split("_")[0] or False
                is_client = len(file_name.split("_")) > 1 and "c" in file_name.split("_")[0] or False
                if is_client:
                    parse_exl_client(os.path.join(root, file), table_name)

                if is_server:
                    parse_exl_server(os.path.join(root, file), table_name, sql_file)

    sql_file.close()
    # if outputAll == 0:
    #     for i in range(len(a)):
    #         ta = a[i].strip('(')
    #         ta = ta.strip(')')
    #         ta = ta.strip(' ')
    #         tableName = listTableName[int(ta) - 1]
    #         if needDb == 1:
    #             createDataBase(database, tableName)
    #         parseExl(database, tableName)
    #         f.write(u"require \"" + PATH_LUA_DATA + tableName + "\"\n")
    # else:
    #     print(u"全导出")
    #     for tableName in dictTableConfig.keys():
    #         if needDb == 1:
    #             createDataBase(database, tableName)
    #         parseExl(database, tableName)
    #         f.write(u"require \"" + PATH_LUA_DATA + tableName + "\"\n")

    print(u"导出数据完成啦~ 花费 {} 秒.".format(time.time() - start_time))
