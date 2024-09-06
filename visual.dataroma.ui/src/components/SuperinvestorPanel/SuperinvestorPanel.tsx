import React from "react";
import styles from "./SuperinvestorPanel.module.scss";
import { Superinvestor } from "./SuperinvestorPanel.types";

interface SuperinvestorPanelProps {
    data: Superinvestor[];
}

const SuperinvestorPanel: React.FC<SuperinvestorPanelProps> = ({ data }) => {
    return (
        <div className={styles.container}>
            {data.map((item, i) => (
                <div key={item.id} className={styles.panel}>
                    <div className={styles.manager}>
                        <span>#{i + 1}</span>
                        <img
                            src={`data:image/jpeg;base64,${item.managerBase64}`}
                            alt={item.portfolioManager}
                            className={styles.image}
                        />
                    </div>
                    <div className={styles.content}>
                        <h3>{item.portfolioManager}</h3>
                        <p>Portfolio Value: ${item.portfolioValue.toLocaleString()}</p>
                        <p>Number of Stocks: {item.numberOfStocks}</p>
                        <p>Last Updated: {item.updatedAt.toString()}</p>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default SuperinvestorPanel;