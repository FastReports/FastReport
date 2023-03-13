import React, { PureComponent, Fragment } from 'react';

export class Report extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            options: [
                {
                    value: '-- Select report name --',
                },
                {
                    value: 'Groups',
                },
                {
                    value: 'Master-Detail',

                },
                {
                    value: 'QR-Codes',
                },
                {
                    value: 'Highlight',
                },
                {
                    value: 'Image',
                }
            ]
        };
    }

    handleChange = (event) => {
        this.setState({ value: event.target.value });
        this.name = event.target.value;
    }

     getReport(name) {
         return { __html: '<iframe width="1000" height="1000" src="/Home?reportToLoad='+name+'"/>' }
    }

    render() {
        const { options, value } = this.state;

        return (
            <div>
                <div>
                    <Fragment>
                        <select onChange={this.handleChange} value={value}>
                            {options.map(item => (
                                <option key={item.value} value={item.value}>
                                    {item.value}
                                </option>
                            ))}
                        </select>
                    </Fragment>
                </div>
                <div dangerouslySetInnerHTML={this.name ? this.getReport(this.name) : { __html: '<iframe />' }}/>
            </div>
        );
       }

}
