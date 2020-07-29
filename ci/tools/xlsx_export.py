# coding:utf-8

from __future__ import division
from __future__ import print_function

import codecs
import os
import sys
import time

import xlrd

try:  # python3
    import configparser
except:  # python2
    import ConfigParser as configparser

reload(sys)
sys.setdefaultencoding('utf8')

PATH_JSON_DATA = "JsonData/"
PATH_XLSX = "xlsx/"


def parse_exl(exl_path, table_name):
    f = codecs.open(os.path.join(json_output, table_name) + ".json", "w", "utf-8")
    # f.write(u"local tableName = {\n\t\"items\":[\n")

    print(exl_path)
    workbook = xlrd.open_workbook(exl_path)
    # 获取sheet
    sheet_name = workbook.sheet_names()[0]
    sheet = workbook.sheet_by_name(sheet_name)

    print(u"开始导出" + table_name)

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
    try:
        if os.path.exists(json_output):
            os.remove(json_output)

        os.makedirs(json_output)
    except Exception as e:
        pass

    for root, dirs, files in os.walk(xlsx_input):
        for file in files:
            parse_exl(os.path.join(root, file), file.split(".")[0])

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
