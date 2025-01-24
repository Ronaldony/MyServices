import axios from "axios";
import { Fragment, useEffect, useState } from "react";
import React from "react";

export default function TestPage()
{
    useEffect(() => {
        // axios.get(`${process.env.REACT_APP_API_BASE}${"/test/class-manager/create-schema?schemaName=testSchema&category=testCate&desc=test"}`)
        axios.get(`${process.env.REACT_APP_API_BASE}${"/test/class-manager/find-schema?schemaName=testSchema"}`)
        .then(res => {
            console.log(res.data)
        }).catch(err => {
            console.log(err)
        })
    }, [])

    console.log(`${process.env.REACT_APP_API_BASE}${process.env.REACT_APP_API_SUB_SERVER_SATUS}`)

    return (
        <Fragment>
            <div>
                Configuration page.
            </div>
        </Fragment>
    )
}